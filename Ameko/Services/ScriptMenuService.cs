using Avalonia.Controls;
using Avalonia.Svg.Skia;
using Holo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ameko.Services
{
    public class ScriptMenuService
    {
        public static List<MenuItem> GenerateScriptMenuItemSource(ICommand ActivateScriptCommand)
        {
            List<MenuItem> congregation = new List<MenuItem>();

            var submenuItemsMap = new Dictionary<string, List<MenuItem>>();
            var rootItems = new List<MenuItem>();
            var groups = new List<MenuItem>();
            var overrides = HoloContext.Instance.ConfigurationManager.SubmenuOverridesMap;

            var scripts = ScriptService.Instance.HoloScripts;

            foreach (var script in scripts )
            {
                if (script.QualifiedName == null) continue;
                var scriptSvg = new Avalonia.Svg.Skia.Svg(new Uri("avares://Ameko/Assets/B5/code-slash.svg")) { Path = new Uri("avares://Ameko/Assets/B5/code-slash.svg").LocalPath };

                var mi = new MenuItem
                {
                    Header = script.Name,
                    Command = ActivateScriptCommand,
                    CommandParameter = script.QualifiedName,
                    Icon = scriptSvg
                };

                // Manual override
                if (overrides.ContainsKey(script.QualifiedName))
                {
                    var specified = overrides[script.QualifiedName];
                    if (specified.Equals("-")) // Root override
                    {
                        rootItems.Add(mi);
                    }
                    else
                    {
                        if (!submenuItemsMap.ContainsKey(specified))
                            submenuItemsMap[specified] = new List<MenuItem>();
                        submenuItemsMap[specified].Add(mi);
                    }
                }
                // Script set
                else if (script.SubmenuName != null)
                {
                    var specified = script.SubmenuName;
                    if (!submenuItemsMap.ContainsKey(specified))
                        submenuItemsMap[specified] = new List<MenuItem>();
                    submenuItemsMap[specified].Add(mi);
                }
                // Root
                else
                {
                    rootItems.Add(mi);
                }
            }

            foreach (var subKV in submenuItemsMap)
            {
                var group = new MenuItem
                {
                    Header = subKV.Key,
                    ItemsSource = subKV.Value
                };
                groups.Add(group);
            }

            congregation.AddRange(groups);
            congregation.AddRange(rootItems);
            return congregation;
        }
    }
}
