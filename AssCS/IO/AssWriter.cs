﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AssCS.IO
{
    public class AssWriter : IFileWriter
    {
        private static readonly string EVENT_FORMAT = "Format: Layer, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text";
        private static readonly string STYLE_FORMAT_V400P = "Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, OutlineColour, " +
            "BackColour, Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, Encoding";

        private readonly File file;
        private readonly string filepath;
        private readonly Encoding encoding;
        private readonly ConsumerInfo consumer;

        public void Write(bool export)
        {
            using var ioFile = System.IO.File.Open(filepath, System.IO.FileMode.Open);
            var writer = new System.IO.StreamWriter(ioFile, encoding);

            WriteHeader(writer);
            WriteScriptInfo(writer);
            if (!export) WriteProjectProperties(writer);
            WriteStyles(writer);
            WriteAttachments(writer);
            WriteEvents(writer);
            if (!export) WriteExtradata(writer);

            writer.Close();
            ioFile.Close();
        }

        private void WriteHeader(StreamWriter writer)
        {
            writer.WriteLine("[Script Info]");
            writer.WriteLine($"; Script generated by {consumer.Name} {consumer.Version} ({AssCSInfo.Name} {AssCSInfo.Version})");
            writer.WriteLine($"; {consumer.Website} ({AssCSInfo.Website})");
        }

        private void WriteScriptInfo(StreamWriter writer)
        {
            foreach (var info in file.InfoManager.GetAll())
            {
                writer.WriteLine($"{info.Key}: {info.Value}");
            }
            writer.WriteLine();
        }

        private void WriteProjectProperties(StreamWriter writer)
        {
            writer.WriteLine("[Aegisub Project Garbage]");
            foreach (var prop in file.PropertiesManager.GetAll())
            {
                writer.WriteLine($"{prop.Name}: {prop.Value}");
            }
            writer.WriteLine();
        }

        private void WriteStyles(StreamWriter writer)
        {
            string head = file.Version switch
            {
                FileVersion.V400 => "[V4 Styles]",
                FileVersion.V400P => "[V4+ Styles]",
                FileVersion.V400PP => "[V4++ Styles]",
                _ => "[V4+ Styles]"
            };
            writer.WriteLine(head);
            writer.WriteLine(STYLE_FORMAT_V400P); // TODO: Versioning
            foreach (var style in file.StyleManager.GetAll())
            {
                writer.WriteLine(style.AsAss()); // TODO: Versioning
            }
            writer.WriteLine();
        }

        private void WriteAttachments(StreamWriter writer)
        {
            // TODO: Attachments
            // writer.WriteLine();
        }

        private void WriteEvents(StreamWriter writer)
        {
            writer.WriteLine("[Events]");
            writer.WriteLine(EVENT_FORMAT);
            foreach (var evnt in file.EventManager.Ordered)
            {
                writer.WriteLine(evnt.AsAss());
            }
            writer.WriteLine();
        }

        private void WriteExtradata(StreamWriter writer)
        {
            writer.WriteLine("[Aegisub Extradata]");
            foreach (var extra in file.ExtradataManager.GetAll())
            {
                string line = $"Data: {extra.Id},{Utilities.InlineEncode(extra.Key)},";
                string inlineEncoded = Utilities.InlineEncode(extra.Value);
                if (4 * extra.Value.Length < 3 * inlineEncoded.Length)
                {
                    line += $"u{Utilities.UUEncode(extra.Value, false)}";
                }
                else
                {
                    line += $"e{inlineEncoded}";
                }
                writer.WriteLine(line);
            }
        }

        public AssWriter(File file, string filepath, Encoding encoding, ConsumerInfo consumer)
        {
            this.file = file;
            this.filepath = filepath;
            this.encoding = encoding;
            this.consumer = consumer;
        }
    }
}
