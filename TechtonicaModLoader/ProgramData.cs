using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Documents;
using System.Xaml;

namespace TechtonicaModLoader
{
    public static class ProgramData
    {
        public static bool isDebugBuild {
            get {
                #if DEBUG
                    return true;
                #else
                    return false;
                #endif
            }
        }
        public static bool isRuntime => MainWindow.current != null;
        public static bool runUnitTests = true;
        public static bool logDebugMessages = true;
        public static string programName = "Techtonica Mod Loader";
        public static bool safeToSave = false;
        public const string bepInExID = "b9a5a1bd-81d8-4913-a46e-70ca7734628c";
        public static ModListSortOption currentSortOption = StringUtils.GetModListSortOptionFromName(Settings.userSettings.defaultSortOption.defaultValue);

        public static int programVersion = 2;
        public static int majorVersion = 0;
        public static int minorVersion = 0;
        public static string versionText => $"{programVersion}.{majorVersion}.{minorVersion}";

        public static int programWidth = 1600;
        public static int programHeight = 675;

        public static class FilePaths
        {
            public static string rootFolder {
                get {
                    if (!isDebugBuild) {
                        return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\TechtonicaModLoaderData";
                    }
                    else {
                        return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\TechtonicaModLoaderDataDebug";
                    }
                }
            }

            // Folders
            public static string resourcesFolder = $"{rootFolder}\\Resources";
            public static string dataFolder = $"{rootFolder}\\Data";
            public static string logsFolder = $"{rootFolder}\\Logs";
            public static string crashReportsFolder = $"{rootFolder}\\CrashReports";
            public static string modsFolder = $"{dataFolder}\\Mods";
            public static string unzipFolder = $"{rootFolder}\\Unzip";
            public static string markdownFiles = $"{dataFolder}\\MarkdownFiles";
            public static string imageCache = $"{rootFolder}\\ImageCache";

            // Files
            public static string logFile = $"{logsFolder}\\TechtonicaModLoader.log";
            public static string settingsFile = $"{dataFolder}\\Settings.json";
            public static string modsSaveFile = $"{dataFolder}\\Mods.json";
            public static string profilesSaveFile = $"{dataFolder}\\Profiles.json";
            public static string tempZipFile = $"{dataFolder}\\Temp.zip";
            public static string tempMarkdownFile = $"{dataFolder}\\README.md";
            public static string imageCacheMapFile = $"{dataFolder}\\ImageCacheMap.json";

            public static string gameFolder => Settings.userSettings.gameFolder.value;
            public static string gameDataFolder = $"{gameFolder}/Techtonica_Data";
            public static string bepInExConfigFolder => $"{gameFolder}/BepInEx/config";
            public static string bepInExPatchersFolder => $"{gameFolder}/BepInEx/patchers";
            public static string bepInExPluginsFolder => $"{gameFolder}/BepInEx/plugins";

            // Public Functions

            public static void CreateFolderStructure() {
                List<string> folders = new List<string> {
                    rootFolder,
                    resourcesFolder,
                    $"{resourcesFolder}\\ControlBox",
                    $"{resourcesFolder}\\GUI",
                    $"{resourcesFolder}\\OnlineModPanel",
                    $"{resourcesFolder}\\InstalledModPanel",
                    dataFolder,
                    logsFolder,
                    crashReportsFolder,
                    modsFolder,
                    unzipFolder,
                    markdownFiles,
                    imageCache
                };

                foreach (string folder in folders) {
                    Directory.CreateDirectory(folder);
                }
            }

            public static void GenerateResources() {
                // ControlBox
                GenerateSettingsSVG();
                GenerateMoveSVG();
                GenerateMinimiseSVG();
                GenerateCloseSVG();

                // GUI
                GenerateUpSVG();
                GenerateDownSVG();
                GenerateInfoSVG();
                GenerateWarningSVG();
                GenerateErrorSVG();

                // Online Mod Panel
                GenerateDownloadSVG();

                // Installed Mod Panel
                GenerateUpdateSVG();
                GenerateDonateSVG();
                GenerateConfigureSVG();
                GenerateViewModPageSVG();
                GenerateDeleteSVG();

                MainWindow.current.controlBox.RefreshIcons();
            }

            // Private Functions

            private static void GenerateSVG(string name, string svg) {
                string path = $"{resourcesFolder}/{name}.svg";
                if (File.Exists(path)) return;
                File.WriteAllText(path, svg);
            }

            // Control Box SVGs

            private static void GenerateSettingsSVG() {
                string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<svg
   xmlns:dc=""http://purl.org/dc/elements/1.1/""
   xmlns:cc=""http://creativecommons.org/ns#""
   xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#""
   xmlns:svg=""http://www.w3.org/2000/svg""
   xmlns=""http://www.w3.org/2000/svg""
   xmlns:sodipodi=""http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd""
   xmlns:inkscape=""http://www.inkscape.org/namespaces/inkscape""
   id=""Layer_1""
   enable-background=""new 0 0 512 512""
   height=""512""
   viewBox=""0 0 512 512""
   width=""512""
   version=""1.1""
   sodipodi:docname=""Settings.svg""
   inkscape:version=""1.0.2-2 (e86c870879, 2021-01-15)"">
  <metadata
     id=""metadata11"">
    <rdf:RDF>
      <cc:Work
         rdf:about="""">
        <dc:format>image/svg+xml</dc:format>
        <dc:type
           rdf:resource=""http://purl.org/dc/dcmitype/StillImage"" />
      </cc:Work>
    </rdf:RDF>
  </metadata>
  <defs
     id=""defs9"" />
  <sodipodi:namedview
     pagecolor=""#ffffff""
     bordercolor=""#666666""
     borderopacity=""1""
     objecttolerance=""10""
     gridtolerance=""10""
     guidetolerance=""10""
     inkscape:pageopacity=""0""
     inkscape:pageshadow=""2""
     inkscape:window-width=""1920""
     inkscape:window-height=""1017""
     id=""namedview7""
     showgrid=""false""
     inkscape:zoom=""1.6953125""
     inkscape:cx=""256""
     inkscape:cy=""256""
     inkscape:window-x=""-8""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""Layer_1"" />
  <path
     d=""m272.066 512h-32.133c-25.989 0-47.134-21.144-47.134-47.133v-10.871c-11.049-3.53-21.784-7.986-32.097-13.323l-7.704 7.704c-18.659 18.682-48.548 18.134-66.665-.007l-22.711-22.71c-18.149-18.129-18.671-48.008.006-66.665l7.698-7.698c-5.337-10.313-9.792-21.046-13.323-32.097h-10.87c-25.988 0-47.133-21.144-47.133-47.133v-32.134c0-25.989 21.145-47.133 47.134-47.133h10.87c3.531-11.05 7.986-21.784 13.323-32.097l-7.704-7.703c-18.666-18.646-18.151-48.528.006-66.665l22.713-22.712c18.159-18.184 48.041-18.638 66.664.006l7.697 7.697c10.313-5.336 21.048-9.792 32.097-13.323v-10.87c0-25.989 21.144-47.133 47.134-47.133h32.133c25.989 0 47.133 21.144 47.133 47.133v10.871c11.049 3.53 21.784 7.986 32.097 13.323l7.704-7.704c18.659-18.682 48.548-18.134 66.665.007l22.711 22.71c18.149 18.129 18.671 48.008-.006 66.665l-7.698 7.698c5.337 10.313 9.792 21.046 13.323 32.097h10.87c25.989 0 47.134 21.144 47.134 47.133v32.134c0 25.989-21.145 47.133-47.134 47.133h-10.87c-3.531 11.05-7.986 21.784-13.323 32.097l7.704 7.704c18.666 18.646 18.151 48.528-.006 66.665l-22.713 22.712c-18.159 18.184-48.041 18.638-66.664-.006l-7.697-7.697c-10.313 5.336-21.048 9.792-32.097 13.323v10.871c0 25.987-21.144 47.131-47.134 47.131zm-106.349-102.83c14.327 8.473 29.747 14.874 45.831 19.025 6.624 1.709 11.252 7.683 11.252 14.524v22.148c0 9.447 7.687 17.133 17.134 17.133h32.133c9.447 0 17.134-7.686 17.134-17.133v-22.148c0-6.841 4.628-12.815 11.252-14.524 16.084-4.151 31.504-10.552 45.831-19.025 5.895-3.486 13.4-2.538 18.243 2.305l15.688 15.689c6.764 6.772 17.626 6.615 24.224.007l22.727-22.726c6.582-6.574 6.802-17.438.006-24.225l-15.695-15.695c-4.842-4.842-5.79-12.348-2.305-18.242 8.473-14.326 14.873-29.746 19.024-45.831 1.71-6.624 7.684-11.251 14.524-11.251h22.147c9.447 0 17.134-7.686 17.134-17.133v-32.134c0-9.447-7.687-17.133-17.134-17.133h-22.147c-6.841 0-12.814-4.628-14.524-11.251-4.151-16.085-10.552-31.505-19.024-45.831-3.485-5.894-2.537-13.4 2.305-18.242l15.689-15.689c6.782-6.774 6.605-17.634.006-24.225l-22.725-22.725c-6.587-6.596-17.451-6.789-24.225-.006l-15.694 15.695c-4.842 4.843-12.35 5.791-18.243 2.305-14.327-8.473-29.747-14.874-45.831-19.025-6.624-1.709-11.252-7.683-11.252-14.524v-22.15c0-9.447-7.687-17.133-17.134-17.133h-32.133c-9.447 0-17.134 7.686-17.134 17.133v22.148c0 6.841-4.628 12.815-11.252 14.524-16.084 4.151-31.504 10.552-45.831 19.025-5.896 3.485-13.401 2.537-18.243-2.305l-15.688-15.689c-6.764-6.772-17.627-6.615-24.224-.007l-22.727 22.726c-6.582 6.574-6.802 17.437-.006 24.225l15.695 15.695c4.842 4.842 5.79 12.348 2.305 18.242-8.473 14.326-14.873 29.746-19.024 45.831-1.71 6.624-7.684 11.251-14.524 11.251h-22.148c-9.447.001-17.134 7.687-17.134 17.134v32.134c0 9.447 7.687 17.133 17.134 17.133h22.147c6.841 0 12.814 4.628 14.524 11.251 4.151 16.085 10.552 31.505 19.024 45.831 3.485 5.894 2.537 13.4-2.305 18.242l-15.689 15.689c-6.782 6.774-6.605 17.634-.006 24.225l22.725 22.725c6.587 6.596 17.451 6.789 24.225.006l15.694-15.695c3.568-3.567 10.991-6.594 18.244-2.304z""
     id=""path2""
     style=""fill:#ffffff;fill-opacity:1"" />
  <path
     d=""m256 367.4c-61.427 0-111.4-49.974-111.4-111.4s49.973-111.4 111.4-111.4 111.4 49.974 111.4 111.4-49.973 111.4-111.4 111.4zm0-192.8c-44.885 0-81.4 36.516-81.4 81.4s36.516 81.4 81.4 81.4 81.4-36.516 81.4-81.4-36.515-81.4-81.4-81.4z""
     id=""path4""
     style=""fill:#ffffff;fill-opacity:1"" />
</svg>
";
                GenerateSVG("ControlBox/Settings", svg);
            }

            private static void GenerateMoveSVG() {
                string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<svg
   xmlns:dc=""http://purl.org/dc/elements/1.1/""
   xmlns:cc=""http://creativecommons.org/ns#""
   xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#""
   xmlns:svg=""http://www.w3.org/2000/svg""
   xmlns=""http://www.w3.org/2000/svg""
   xmlns:sodipodi=""http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd""
   xmlns:inkscape=""http://www.inkscape.org/namespaces/inkscape""
   version=""1.1""
   id=""Layer_1""
   x=""0px""
   y=""0px""
   viewBox=""0 0 492.009 492.009""
   style=""enable-background:new 0 0 492.009 492.009;""
   xml:space=""preserve""
   sodipodi:docname=""Move.svg""
   inkscape:version=""1.0.2-2 (e86c870879, 2021-01-15)""><metadata
   id=""metadata67""><rdf:RDF><cc:Work
       rdf:about=""""><dc:format>image/svg+xml</dc:format><dc:type
         rdf:resource=""http://purl.org/dc/dcmitype/StillImage"" /></cc:Work></rdf:RDF></metadata><defs
   id=""defs65"" /><sodipodi:namedview
   pagecolor=""#ffffff""
   bordercolor=""#666666""
   borderopacity=""1""
   objecttolerance=""10""
   gridtolerance=""10""
   guidetolerance=""10""
   inkscape:pageopacity=""0""
   inkscape:pageshadow=""2""
   inkscape:window-width=""1920""
   inkscape:window-height=""1017""
   id=""namedview63""
   showgrid=""false""
   inkscape:zoom=""1.7641954""
   inkscape:cx=""246.0045""
   inkscape:cy=""246.0045""
   inkscape:window-x=""-8""
   inkscape:window-y=""-8""
   inkscape:window-maximized=""1""
   inkscape:current-layer=""Layer_1"" />
<g
   id=""g6""
   style=""fill:#ffffff;fill-opacity:1"">
	<g
   id=""g4""
   style=""fill:#ffffff;fill-opacity:1"">
		<path
   d=""M314.343,62.977L255.399,4.033c-2.672-2.672-6.236-4.04-9.92-4.032c-3.752-0.036-7.396,1.36-10.068,4.032l-57.728,57.728    c-5.408,5.408-5.408,14.2,0,19.604l7.444,7.444c5.22,5.22,14.332,5.22,19.556,0l22.1-22.148v81.388    c0,0.248,0.144,0.452,0.188,0.684c0.6,7.092,6.548,12.704,13.8,12.704h10.52c7.644,0,13.928-6.208,13.928-13.852v-9.088    c0-0.04,0-0.068,0-0.1V67.869l22.108,22.152c5.408,5.408,14.18,5.408,19.584,0l7.432-7.436    C319.751,77.173,319.751,68.377,314.343,62.977z""
   id=""path2""
   style=""fill:#ffffff;fill-opacity:1"" />
	</g>
</g>
<g
   id=""g12""
   style=""fill:#ffffff;fill-opacity:1"">
	<g
   id=""g10""
   style=""fill:#ffffff;fill-opacity:1"">
		<path
   d=""M314.335,409.437l-7.44-7.456c-5.22-5.228-14.336-5.228-19.564,0l-22.108,22.152v-70.216c0-0.04,0-0.064,0-0.1v-9.088    c0-7.648-6.288-14.16-13.924-14.16h-10.528c-7.244,0-13.192,5.756-13.796,12.856c-0.044,0.236-0.188,0.596-0.188,0.84v81.084    l-22.1-22.148c-5.224-5.224-14.356-5.224-19.58,0l-7.44,7.444c-5.4,5.404-5.392,14.2,0.016,19.608l57.732,57.724    c2.604,2.612,6.08,4.032,9.668,4.032h0.52c3.716,0,7.184-1.416,9.792-4.032l58.94-58.94    C319.743,423.633,319.743,414.841,314.335,409.437z""
   id=""path8""
   style=""fill:#ffffff;fill-opacity:1"" />
	</g>
</g>
<g
   id=""g18""
   style=""fill:#ffffff;fill-opacity:1"">
	<g
   id=""g16""
   style=""fill:#ffffff;fill-opacity:1"">
		<path
   d=""M147.251,226.781l-1.184,0h-7.948c-0.028,0-0.056,0-0.088,0h-69.88l22.152-22.032c2.612-2.608,4.048-6.032,4.048-9.74    c0-3.712-1.436-7.164-4.048-9.768l-7.444-7.428c-5.408-5.408-14.204-5.4-19.604,0.008l-58.944,58.94    c-2.672,2.668-4.1,6.248-4.028,9.92c-0.076,3.82,1.356,7.396,4.028,10.068l57.728,57.732c2.704,2.704,6.252,4.056,9.804,4.056    s7.1-1.352,9.804-4.056l7.44-7.44c2.612-2.608,4.052-6.092,4.052-9.8c0-3.712-1.436-7.232-4.052-9.836l-22.144-22.184h80.728    c0.244,0,0.644-0.06,0.876-0.104c7.096-0.6,12.892-6.468,12.892-13.716v-10.536C161.439,233.229,154.895,226.781,147.251,226.781z    ""
   id=""path14""
   style=""fill:#ffffff;fill-opacity:1"" />
	</g>
</g>
<g
   id=""g24""
   style=""fill:#ffffff;fill-opacity:1"">
	<g
   id=""g22""
   style=""fill:#ffffff;fill-opacity:1"">
		<path
   d=""M487.695,236.765l-58.944-58.936c-5.404-5.408-14.2-5.408-19.604,0l-7.436,7.444c-2.612,2.604-4.052,6.088-4.052,9.796    c0,3.712,1.436,7.072,4.052,9.68l22.148,22.032h-70.328c-0.036,0-0.064,0-0.096,0h-9.084c-7.644,0-13.78,6.444-13.78,14.084    v10.536c0,7.248,5.564,13.108,12.664,13.712c0.236,0.048,0.408,0.108,0.648,0.108h81.188l-22.156,22.18    c-2.608,2.604-4.048,6.116-4.048,9.816c0,3.716,1.436,7.208,4.048,9.816l7.448,7.444c2.7,2.704,6.248,4.06,9.8,4.06    s7.096-1.352,9.8-4.056l57.736-57.732c2.664-2.664,4.092-6.244,4.028-9.92C491.787,243.009,490.359,239.429,487.695,236.765z""
   id=""path20""
   style=""fill:#ffffff;fill-opacity:1"" />
	</g>
</g>
<g
   id=""g30""
   style=""fill:#ffffff;fill-opacity:1"">
	<g
   id=""g28""
   style=""fill:#ffffff;fill-opacity:1"">
		<path
   d=""M246.011,207.541c-21.204,0-38.456,17.252-38.456,38.46c0,21.204,17.252,38.46,38.456,38.46    c21.204,0,38.46-17.256,38.46-38.46C284.471,224.793,267.215,207.541,246.011,207.541z""
   id=""path26""
   style=""fill:#ffffff;fill-opacity:1"" />
	</g>
</g>
<g
   id=""g32""
   style=""fill:#ffffff;fill-opacity:1"">
</g>
<g
   id=""g34""
   style=""fill:#ffffff;fill-opacity:1"">
</g>
<g
   id=""g36""
   style=""fill:#ffffff;fill-opacity:1"">
</g>
<g
   id=""g38""
   style=""fill:#ffffff;fill-opacity:1"">
</g>
<g
   id=""g40""
   style=""fill:#ffffff;fill-opacity:1"">
</g>
<g
   id=""g42""
   style=""fill:#ffffff;fill-opacity:1"">
</g>
<g
   id=""g44""
   style=""fill:#ffffff;fill-opacity:1"">
</g>
<g
   id=""g46""
   style=""fill:#ffffff;fill-opacity:1"">
</g>
<g
   id=""g48""
   style=""fill:#ffffff;fill-opacity:1"">
</g>
<g
   id=""g50""
   style=""fill:#ffffff;fill-opacity:1"">
</g>
<g
   id=""g52""
   style=""fill:#ffffff;fill-opacity:1"">
</g>
<g
   id=""g54""
   style=""fill:#ffffff;fill-opacity:1"">
</g>
<g
   id=""g56""
   style=""fill:#ffffff;fill-opacity:1"">
</g>
<g
   id=""g58""
   style=""fill:#ffffff;fill-opacity:1"">
</g>
<g
   id=""g60""
   style=""fill:#ffffff;fill-opacity:1"">
</g>
</svg>
";
                GenerateSVG("ControlBox/Move", svg);
            }

            private static void GenerateMinimiseSVG() {
                string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg8""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Minimise.svg""
   xmlns:inkscape=""http://www.inkscape.org/namespaces/inkscape""
   xmlns:sodipodi=""http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd""
   xmlns=""http://www.w3.org/2000/svg""
   xmlns:svg=""http://www.w3.org/2000/svg""
   xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#""
   xmlns:cc=""http://creativecommons.org/ns#""
   xmlns:dc=""http://purl.org/dc/elements/1.1/"">
  <defs
     id=""defs2"" />
  <sodipodi:namedview
     id=""base""
     pagecolor=""#232323""
     bordercolor=""#666666""
     borderopacity=""1.0""
     inkscape:pageopacity=""0""
     inkscape:pageshadow=""2""
     inkscape:zoom=""1.4""
     inkscape:cx=""215.35714""
     inkscape:cy=""300.35714""
     inkscape:document-units=""px""
     inkscape:current-layer=""layer1""
     showgrid=""false""
     units=""px""
     inkscape:window-width=""1920""
     inkscape:window-height=""1009""
     inkscape:window-x=""-8""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:document-rotation=""0""
     inkscape:pagecheckerboard=""0"" />
  <metadata
     id=""metadata5"">
    <rdf:RDF>
      <cc:Work
         rdf:about="""">
        <dc:format>image/svg+xml</dc:format>
        <dc:type
           rdf:resource=""http://purl.org/dc/dcmitype/StillImage"" />
      </cc:Work>
    </rdf:RDF>
  </metadata>
  <g
     inkscape:label=""Layer 1""
     inkscape:groupmode=""layer""
     id=""layer1""
     transform=""translate(0,-161.53332)"">
    <rect
       style=""fill:#ffffff;stroke:none;stroke-width:9.4084;stroke-linecap:round;paint-order:fill markers stroke""
       id=""rect1221""
       width=""512""
       height=""51.200001""
       x=""0""
       y=""391.93332""
       ry=""25.6"" />
  </g>
</svg>
";
                GenerateSVG("ControlBox/Minimise", svg);
            }

            private static void GenerateCloseSVG() {
                string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg8""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Close.svg""
   xmlns:inkscape=""http://www.inkscape.org/namespaces/inkscape""
   xmlns:sodipodi=""http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd""
   xmlns=""http://www.w3.org/2000/svg""
   xmlns:svg=""http://www.w3.org/2000/svg""
   xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#""
   xmlns:cc=""http://creativecommons.org/ns#""
   xmlns:dc=""http://purl.org/dc/elements/1.1/"">
  <defs
     id=""defs2"" />
  <sodipodi:namedview
     id=""base""
     pagecolor=""#232323""
     bordercolor=""#666666""
     borderopacity=""1.0""
     inkscape:pageopacity=""0""
     inkscape:pageshadow=""2""
     inkscape:zoom=""0.7""
     inkscape:cx=""-73.571429""
     inkscape:cy=""480.71429""
     inkscape:document-units=""px""
     inkscape:current-layer=""layer1""
     showgrid=""false""
     units=""px""
     inkscape:window-width=""1920""
     inkscape:window-height=""1009""
     inkscape:window-x=""-8""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:document-rotation=""0""
     inkscape:pagecheckerboard=""0"" />
  <metadata
     id=""metadata5"">
    <rdf:RDF>
      <cc:Work
         rdf:about="""">
        <dc:format>image/svg+xml</dc:format>
        <dc:type
           rdf:resource=""http://purl.org/dc/dcmitype/StillImage"" />
      </cc:Work>
    </rdf:RDF>
  </metadata>
  <g
     inkscape:label=""Layer 1""
     inkscape:groupmode=""layer""
     id=""layer1""
     transform=""translate(0,-161.53332)"">
    <g
       id=""g1431""
       transform=""matrix(0.96010753,0.96010753,-0.96010753,0.96010753,411.08936,-229.1311)"">
      <rect
         style=""fill:#ffffff;stroke:none;stroke-width:9.4084;stroke-linecap:round;paint-order:fill markers stroke""
         id=""rect1221""
         width=""512""
         height=""51.200001""
         x=""0""
         y=""391.93332""
         ry=""25.6"" />
      <rect
         style=""fill:#ffffff;stroke:none;stroke-width:9.4084;stroke-linecap:round;paint-order:fill markers stroke""
         id=""rect1221-5""
         width=""512""
         height=""51.200001""
         x=""161.53333""
         y=""-281.60001""
         ry=""25.6""
         transform=""rotate(90)"" />
    </g>
  </g>
</svg>
";
                GenerateSVG("ControlBox/Close", svg);
            }

            // GUI SVGs

            private static void GenerateUpSVG() {
                string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Up.svg""
   xmlns:inkscape=""http://www.inkscape.org/namespaces/inkscape""
   xmlns:sodipodi=""http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd""
   xmlns=""http://www.w3.org/2000/svg""
   xmlns:svg=""http://www.w3.org/2000/svg"">
  <sodipodi:namedview
     id=""namedview7""
     pagecolor=""#505050""
     bordercolor=""#eeeeee""
     borderopacity=""1""
     inkscape:pageshadow=""0""
     inkscape:pageopacity=""0""
     inkscape:pagecheckerboard=""0""
     inkscape:document-units=""px""
     showgrid=""false""
     units=""px""
     showguides=""true""
     inkscape:guide-bbox=""true""
     inkscape:zoom=""0.83007813""
     inkscape:cx=""-268.04706""
     inkscape:cy=""313.82588""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""1912""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""0,346.95529""
       orientation=""-1,0""
       id=""guide925""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""256,496.33882""
       orientation=""-1,0""
       id=""guide927""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""512,420.44235""
       orientation=""-1,0""
       id=""guide929""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""-524.04706,512""
       orientation=""0,1""
       id=""guide931""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""-357.79765,0""
       orientation=""0,1""
       id=""guide933""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
  </sodipodi:namedview>
  <defs
     id=""defs2"" />
  <g
     inkscape:label=""Layer 1""
     inkscape:groupmode=""layer""
     id=""layer1"">
    <path
       style=""fill:#ffffff;stroke:none;stroke-width:1px;stroke-linecap:butt;stroke-linejoin:miter;stroke-opacity:1;fill-opacity:1""
       d=""M 0,512 256,0 512,512 H 0""
       id=""path968"" />
  </g>
</svg>
";
                GenerateSVG("GUI/Up", svg);
            }

            private static void GenerateDownSVG() {
                string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Down.svg""
   xmlns:inkscape=""http://www.inkscape.org/namespaces/inkscape""
   xmlns:sodipodi=""http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd""
   xmlns=""http://www.w3.org/2000/svg""
   xmlns:svg=""http://www.w3.org/2000/svg"">
  <sodipodi:namedview
     id=""namedview7""
     pagecolor=""#505050""
     bordercolor=""#eeeeee""
     borderopacity=""1""
     inkscape:pageshadow=""0""
     inkscape:pageopacity=""0""
     inkscape:pagecheckerboard=""0""
     inkscape:document-units=""px""
     showgrid=""false""
     units=""px""
     showguides=""true""
     inkscape:guide-bbox=""true""
     inkscape:zoom=""0.83007813""
     inkscape:cx=""-268.04706""
     inkscape:cy=""313.82588""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""1912""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""0,346.95529""
       orientation=""-1,0""
       id=""guide925""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""256,496.33882""
       orientation=""-1,0""
       id=""guide927""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""512,420.44235""
       orientation=""-1,0""
       id=""guide929""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""-524.04706,512""
       orientation=""0,1""
       id=""guide931""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""-357.79765,0""
       orientation=""0,1""
       id=""guide933""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
  </sodipodi:namedview>
  <defs
     id=""defs2"" />
  <g
     inkscape:label=""Layer 1""
     inkscape:groupmode=""layer""
     id=""layer1"">
    <path
       style=""fill:#ffffff;fill-opacity:1;stroke:none;stroke-width:1px;stroke-linecap:butt;stroke-linejoin:miter;stroke-opacity:1""
       d=""M 0,0 256,512 512,0 H 0""
       id=""path968"" />
  </g>
</svg>
";
                GenerateSVG("GUI/Down", svg);
            }

            private static void GenerateInfoSVG() {
                string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Info.svg""
   xmlns:inkscape=""http://www.inkscape.org/namespaces/inkscape""
   xmlns:sodipodi=""http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd""
   xmlns=""http://www.w3.org/2000/svg""
   xmlns:svg=""http://www.w3.org/2000/svg"">
  <sodipodi:namedview
     id=""namedview7""
     pagecolor=""#505050""
     bordercolor=""#eeeeee""
     borderopacity=""1""
     inkscape:pageshadow=""0""
     inkscape:pageopacity=""0""
     inkscape:pagecheckerboard=""0""
     inkscape:document-units=""px""
     showgrid=""false""
     units=""px""
     showguides=""true""
     inkscape:guide-bbox=""true""
     inkscape:zoom=""1.138""
     inkscape:cx=""482.86467""
     inkscape:cy=""235.94025""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""-8""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""0,45.980582""
       orientation=""-1,0""
       id=""guide989""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""-18.640777,0""
       orientation=""0,1""
       id=""guide991""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""512,0""
       orientation=""-1,0""
       id=""guide1069""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""256,535.72584""
       orientation=""-1,0""
       id=""guide1109""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""231.10721,443.405""
       orientation=""0,1""
       id=""guide1111""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
  </sodipodi:namedview>
  <defs
     id=""defs2"" />
  <g
     inkscape:label=""Layer 1""
     inkscape:groupmode=""layer""
     id=""layer1"">
    <path
       style=""fill:none;fill-opacity:1;stroke:#ffffff;stroke-width:7.91742;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
       d=""M 6.8566886,473.74379 256,42.214922 505.14331,473.74379 Z""
       id=""path854""
       sodipodi:nodetypes=""cccc"" />
    <text
       xml:space=""preserve""
       style=""font-style:normal;font-weight:normal;font-size:421.78px;line-height:1.25;font-family:sans-serif;fill:#ffffff;fill-opacity:1;stroke:#ffffff;stroke-width:10.5445;stroke-opacity:1""
       x=""198.12881""
       y=""442.35748""
       id=""text2077""><tspan
         sodipodi:role=""line""
         id=""tspan2075""
         x=""198.12881""
         y=""442.35748""
         style=""fill:#ffffff;fill-opacity:1;stroke:#ffffff;stroke-width:10.5445;stroke-opacity:1"">i</tspan></text>
  </g>
</svg>
";
                GenerateSVG("GUI/Info", svg);
            }

            private static void GenerateWarningSVG() {
                string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Warning.svg""
   xmlns:inkscape=""http://www.inkscape.org/namespaces/inkscape""
   xmlns:sodipodi=""http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd""
   xmlns=""http://www.w3.org/2000/svg""
   xmlns:svg=""http://www.w3.org/2000/svg"">
  <sodipodi:namedview
     id=""namedview7""
     pagecolor=""#505050""
     bordercolor=""#eeeeee""
     borderopacity=""1""
     inkscape:pageshadow=""0""
     inkscape:pageopacity=""0""
     inkscape:pagecheckerboard=""0""
     inkscape:document-units=""px""
     showgrid=""false""
     units=""px""
     showguides=""true""
     inkscape:guide-bbox=""true""
     inkscape:zoom=""1.138""
     inkscape:cx=""482.86467""
     inkscape:cy=""235.94025""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""-8""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""0,45.980582""
       orientation=""-1,0""
       id=""guide989""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""-18.640777,0""
       orientation=""0,1""
       id=""guide991""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""512,0""
       orientation=""-1,0""
       id=""guide1069""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""256,535.72584""
       orientation=""-1,0""
       id=""guide1109""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""231.10721,443.405""
       orientation=""0,1""
       id=""guide1111""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
  </sodipodi:namedview>
  <defs
     id=""defs2"" />
  <g
     inkscape:label=""Layer 1""
     inkscape:groupmode=""layer""
     id=""layer1"">
    <path
       style=""fill:none;fill-opacity:1;stroke:#fff00f;stroke-width:7.91742;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
       d=""M 6.8566878,473.74379 256,42.214924 505.14331,473.74379 Z""
       id=""path854""
       sodipodi:nodetypes=""cccc"" />
    <text
       xml:space=""preserve""
       style=""font-style:normal;font-weight:normal;font-size:421.78px;line-height:1.25;font-family:sans-serif;fill:#fff00f;fill-opacity:1;stroke:#fff00f;stroke-width:10.5445;stroke-opacity:1""
       x=""-313.87119""
       y=""-133.64253""
       id=""text2077""
       transform=""scale(-1)""><tspan
         sodipodi:role=""line""
         id=""tspan2075""
         x=""-313.87119""
         y=""-133.64253""
         style=""fill:#fff00f;fill-opacity:1;stroke:#fff00f;stroke-width:10.5445;stroke-opacity:1"">i</tspan></text>
  </g>
</svg>
";
                GenerateSVG("GUI/Warning", svg);
            }

            private static void GenerateErrorSVG() {
                string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Error.svg""
   xmlns:inkscape=""http://www.inkscape.org/namespaces/inkscape""
   xmlns:sodipodi=""http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd""
   xmlns:xlink=""http://www.w3.org/1999/xlink""
   xmlns=""http://www.w3.org/2000/svg""
   xmlns:svg=""http://www.w3.org/2000/svg"">
  <sodipodi:namedview
     id=""namedview7""
     pagecolor=""#505050""
     bordercolor=""#eeeeee""
     borderopacity=""1""
     inkscape:pageshadow=""0""
     inkscape:pageopacity=""0""
     inkscape:pagecheckerboard=""0""
     inkscape:document-units=""px""
     showgrid=""false""
     units=""px""
     showguides=""true""
     inkscape:guide-bbox=""true""
     inkscape:zoom=""1.138""
     inkscape:cx=""481.98595""
     inkscape:cy=""235.94025""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""-8""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""0,45.980582""
       orientation=""-1,0""
       id=""guide989""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""-18.640777,0""
       orientation=""0,1""
       id=""guide991""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""512,0""
       orientation=""-1,0""
       id=""guide1069""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""256,535.72584""
       orientation=""-1,0""
       id=""guide1109""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""231.10721,443.405""
       orientation=""0,1""
       id=""guide1111""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
  </sodipodi:namedview>
  <defs
     id=""defs2"">
    <linearGradient
       inkscape:collect=""always""
       id=""linearGradient7077"">
      <stop
         style=""stop-color:#ff0000;stop-opacity:1;""
         offset=""0""
         id=""stop7073"" />
      <stop
         style=""stop-color:#ff0000;stop-opacity:0;""
         offset=""1""
         id=""stop7075"" />
    </linearGradient>
    <linearGradient
       inkscape:collect=""always""
       xlink:href=""#linearGradient7077""
       id=""linearGradient7079""
       x1=""-283.10266""
       y1=""-288.00001""
       x2=""-228.89734""
       y2=""-288.00001""
       gradientUnits=""userSpaceOnUse"" />
  </defs>
  <g
     inkscape:label=""Layer 1""
     inkscape:groupmode=""layer""
     id=""layer1"">
    <path
       style=""fill:none;fill-opacity:1;stroke:#ff0000;stroke-width:7.91742;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
       d=""M 6.8566886,473.74379 256,42.214922 505.14331,473.74379 Z""
       id=""path854""
       sodipodi:nodetypes=""cccc"" />
    <text
       xml:space=""preserve""
       style=""font-style:normal;font-weight:normal;font-size:421.78px;line-height:1.25;font-family:sans-serif;fill:#ff0000;fill-opacity:1;stroke:#ff0000;stroke-width:10.5445;stroke-opacity:1""
       x=""-313.87119""
       y=""-133.64253""
       id=""text2077""
       transform=""scale(-1)""><tspan
         sodipodi:role=""line""
         id=""tspan2075""
         x=""-313.87119""
         y=""-133.64253""
         style=""fill:#ff0000;fill-opacity:1;stroke:#ff0000;stroke-width:10.5445;stroke-opacity:1"">i</tspan></text>
  </g>
</svg>
";
                GenerateSVG("GUI/Error", svg);
            }


            private static void GenerateUpdateSVG() {
                string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Update.svg""
   xmlns:inkscape=""http://www.inkscape.org/namespaces/inkscape""
   xmlns:sodipodi=""http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd""
   xmlns=""http://www.w3.org/2000/svg""
   xmlns:svg=""http://www.w3.org/2000/svg"">
  <sodipodi:namedview
     id=""namedview7""
     pagecolor=""#505050""
     bordercolor=""#eeeeee""
     borderopacity=""1""
     inkscape:pageshadow=""0""
     inkscape:pageopacity=""0""
     inkscape:pagecheckerboard=""0""
     inkscape:document-units=""px""
     showgrid=""false""
     units=""px""
     showguides=""true""
     inkscape:guide-bbox=""true""
     inkscape:zoom=""1.1739077""
     inkscape:cx=""126.50057""
     inkscape:cy=""289.20501""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""1912""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""434.94509,331.56314""
       orientation=""0,-1""
       id=""guide1388"" />
    <sodipodi:guide
       position=""256,311.38798""
       orientation=""-1,0""
       id=""guide1390""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
  </sodipodi:namedview>
  <defs
     id=""defs2"" />
  <g
     inkscape:label=""Layer 1""
     inkscape:groupmode=""layer""
     id=""layer1"">
    <path
       id=""rect843""
       style=""fill:#ffffff;stroke:none;stroke-width:2.85941;stroke-linecap:square;paint-order:fill markers stroke;fill-opacity:1""
       d=""m 256.0521,1.4393963 178.89297,178.8929737 2e-5,0.10449 -357.890451,-1e-5 v -0.10451 L 255.94761,1.4393715 c 0.0289,-0.028935 0.0755,-0.02897 0.10449,2.12e-5 z""
       sodipodi:nodetypes=""cccccssc"" />
    <rect
       style=""fill:#ffffff;stroke:none;stroke-width:2.87977;stroke-linecap:square;paint-order:fill markers stroke;fill-opacity:1""
       id=""rect1306""
       width=""125.12022""
       height=""330.09476""
       x=""193.4399""
       y=""180.47275""
       ry=""0.076415896"" />
  </g>
</svg>
";
                GenerateSVG("InstalledModPanel/Update", svg);
            }

            private static void GenerateDonateSVG() {
                string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Donate.svg""
   xmlns:inkscape=""http://www.inkscape.org/namespaces/inkscape""
   xmlns:sodipodi=""http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd""
   xmlns=""http://www.w3.org/2000/svg""
   xmlns:svg=""http://www.w3.org/2000/svg"">
  <sodipodi:namedview
     id=""namedview7""
     pagecolor=""#505050""
     bordercolor=""#eeeeee""
     borderopacity=""1""
     inkscape:pageshadow=""0""
     inkscape:pageopacity=""0""
     inkscape:pagecheckerboard=""0""
     inkscape:document-units=""px""
     showgrid=""false""
     units=""px""
     showguides=""true""
     inkscape:guide-bbox=""true""
     inkscape:zoom=""1.1739077""
     inkscape:cx=""56.648404""
     inkscape:cy=""324.98295""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""1912""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""434.94509,331.56314""
       orientation=""0,-1""
       id=""guide1388"" />
    <sodipodi:guide
       position=""256,311.38798""
       orientation=""-1,0""
       id=""guide1390""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
  </sodipodi:namedview>
  <defs
     id=""defs2"" />
  <g
     inkscape:label=""Layer 1""
     inkscape:groupmode=""layer""
     id=""layer1"">
    <g
       aria-label=""♥""
       id=""text6761""
       style=""font-size:835.519px;line-height:1.25;stroke-width:20.888;stroke:none;fill:#ffffff;fill-opacity:1"">
      <path
         d=""M 256.81594,512.00007 Q 244.16892,463.85982 220.50676,421.83909 197.25257,379.41039 129.93781,290.47331 80.573651,225.19839 69.15054,207.65575 50.384001,179.09798 41.816667,155.43582 q -8.159365,-24.07013 -8.159365,-48.54823 0,-45.284471 30.189651,-75.88209 30.189651,-30.59761928 74.658187,-30.59761928 44.87651,0 77.92194,31.82152328 24.88606,23.662159 40.38886,70.578506 Q 270.27889,56.7075 294.75698,32.637373 328.61835,-8.7529421e-5 373.08689,-8.7529421e-5 q 44.06057,0 74.65819,30.597618529421 30.59762,30.189651 30.59762,72.210379 0,36.71714 -17.95061,76.69803 -17.9506,39.57292 -69.3546,104.03191 -66.90679,84.44943 -97.50441,138.7092 -24.07013,42.83667 -36.71714,89.75302 z""
         id=""path7437""
         style=""stroke:none;fill:#ffffff;fill-opacity:1"" />
    </g>
  </g>
</svg>
";
                GenerateSVG("InstalledModPanel/Donate", svg);
            }

            private static void GenerateConfigureSVG() {
                string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Configure.svg""
   xmlns:inkscape=""http://www.inkscape.org/namespaces/inkscape""
   xmlns:sodipodi=""http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd""
   xmlns=""http://www.w3.org/2000/svg""
   xmlns:svg=""http://www.w3.org/2000/svg"">
  <sodipodi:namedview
     id=""namedview7""
     pagecolor=""#505050""
     bordercolor=""#eeeeee""
     borderopacity=""1""
     inkscape:pageshadow=""0""
     inkscape:pageopacity=""0""
     inkscape:pagecheckerboard=""0""
     inkscape:document-units=""px""
     showgrid=""false""
     units=""px""
     showguides=""true""
     inkscape:guide-bbox=""true""
     inkscape:zoom=""0.83007813""
     inkscape:cx=""-216.24471""
     inkscape:cy=""470.43765""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""1912""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""434.94509,256""
       orientation=""0,1""
       id=""guide1388""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""256,311.38798""
       orientation=""-1,0""
       id=""guide1390""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""207.00093,384""
       orientation=""0,1""
       id=""guide8062""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""386.10824,128""
       orientation=""0,1""
       id=""guide8086""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
  </sodipodi:namedview>
  <defs
     id=""defs2"" />
  <g
     inkscape:label=""Layer 1""
     inkscape:groupmode=""layer""
     id=""layer1"">
    <g
       id=""g8999""
       transform=""matrix(1.6,0,0,1.6,-153.6,-153.6)"">
      <circle
         style=""fill:none;fill-opacity:1;stroke:#ffffff;stroke-width:26.9135;stroke-linecap:square;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1;paint-order:fill markers stroke""
         id=""path7876""
         cx=""256""
         cy=""256""
         r=""107.65385"" />
      <g
         id=""g8090"">
        <rect
           style=""fill:#ffffff;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
           id=""rect7980""
           width=""64""
           height=""64""
           x=""224""
           y=""96""
           ry=""0.15772727"" />
        <rect
           style=""fill:#ffffff;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
           id=""rect7980-9""
           width=""64""
           height=""64""
           x=""224""
           y=""352""
           ry=""0.15772727"" />
      </g>
      <g
         id=""g8194"">
        <g
           id=""g8090-6""
           transform=""rotate(30,256,256)"">
          <rect
             style=""fill:#ffffff;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
             id=""rect7980-7""
             width=""64""
             height=""64""
             x=""224""
             y=""96""
             ry=""0.15772727"" />
          <rect
             style=""fill:#ffffff;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
             id=""rect7980-9-7""
             width=""64""
             height=""64""
             x=""224""
             y=""352""
             ry=""0.15772727"" />
        </g>
        <g
           id=""g8090-6-1""
           transform=""rotate(60,256,256)"">
          <rect
             style=""fill:#ffffff;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
             id=""rect7980-7-4""
             width=""64""
             height=""64""
             x=""224""
             y=""96""
             ry=""0.15772727"" />
          <rect
             style=""fill:#ffffff;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
             id=""rect7980-9-7-3""
             width=""64""
             height=""64""
             x=""224""
             y=""352""
             ry=""0.15772727"" />
        </g>
      </g>
      <g
         id=""g8194-1""
         transform=""matrix(-1,0,0,1,512,0)"">
        <g
           id=""g8090-6-6""
           transform=""rotate(30,256,256)"">
          <rect
             style=""fill:#ffffff;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
             id=""rect7980-7-3""
             width=""64""
             height=""64""
             x=""224""
             y=""96""
             ry=""0.15772727"" />
          <rect
             style=""fill:#ffffff;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
             id=""rect7980-9-7-2""
             width=""64""
             height=""64""
             x=""224""
             y=""352""
             ry=""0.15772727"" />
        </g>
        <g
           id=""g8090-6-1-9""
           transform=""rotate(60,256,256)"">
          <rect
             style=""fill:#ffffff;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
             id=""rect7980-7-4-5""
             width=""64""
             height=""64""
             x=""224""
             y=""96""
             ry=""0.15772727"" />
          <rect
             style=""fill:#ffffff;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
             id=""rect7980-9-7-3-0""
             width=""64""
             height=""64""
             x=""224""
             y=""352""
             ry=""0.15772727"" />
        </g>
      </g>
      <g
         id=""g8090-6-1-3""
         transform=""rotate(90,256,256)"">
        <rect
           style=""fill:#ffffff;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
           id=""rect7980-7-4-4""
           width=""64""
           height=""64""
           x=""224""
           y=""96""
           ry=""0.15772727"" />
        <rect
           style=""fill:#ffffff;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
           id=""rect7980-9-7-3-8""
           width=""64""
           height=""64""
           x=""224""
           y=""352""
           ry=""0.15772727"" />
      </g>
    </g>
  </g>
</svg>
";
                GenerateSVG("InstalledModPanel/Configure", svg);
            }

            private static void GenerateViewModPageSVG() {
                string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""ViewModPage.svg""
   xmlns:inkscape=""http://www.inkscape.org/namespaces/inkscape""
   xmlns:sodipodi=""http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd""
   xmlns=""http://www.w3.org/2000/svg""
   xmlns:svg=""http://www.w3.org/2000/svg"">
  <sodipodi:namedview
     id=""namedview7""
     pagecolor=""#505050""
     bordercolor=""#eeeeee""
     borderopacity=""1""
     inkscape:pageshadow=""0""
     inkscape:pageopacity=""0""
     inkscape:pagecheckerboard=""0""
     inkscape:document-units=""px""
     showgrid=""false""
     units=""px""
     showguides=""true""
     inkscape:guide-bbox=""true""
     inkscape:zoom=""1.1739077""
     inkscape:cx=""99.241189""
     inkscape:cy=""267.05676""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""1912""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""434.94509,256""
       orientation=""0,1""
       id=""guide1388""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""256,311.38798""
       orientation=""-1,0""
       id=""guide1390""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""207.00093,384""
       orientation=""0,1""
       id=""guide8062""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""386.10824,128""
       orientation=""0,1""
       id=""guide8086""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""314.42824,673.43059""
       orientation=""0,-1""
       id=""guide9987"" />
    <sodipodi:guide
       position=""360.80941,455.98118""
       orientation=""0,-1""
       id=""guide10173"" />
    <sodipodi:guide
       position=""181.91059,414.41882""
       orientation=""1,0""
       id=""guide10175"" />
    <sodipodi:guide
       position=""324.06588,414.41882""
       orientation=""1,0""
       id=""guide10177"" />
  </sodipodi:namedview>
  <defs
     id=""defs2"" />
  <g
     inkscape:label=""Layer 1""
     inkscape:groupmode=""layer""
     id=""layer1"">
    <g
       id=""g13340"">
      <g
         id=""g13286"">
        <circle
           style=""fill:none;fill-opacity:1;stroke:#ffffff;stroke-width:15.938;stroke-linecap:square;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1;paint-order:fill markers stroke""
           id=""path7876""
           cx=""256""
           cy=""256""
           r=""248.03113"" />
        <path
           style=""fill:none;stroke:#ffffff;stroke-width:16;stroke-linecap:butt;stroke-linejoin:miter;stroke-opacity:1;stroke-miterlimit:4;stroke-dasharray:none""
           d=""M 8.4329412,256 H 503.56706""
           id=""path10022"" />
        <path
           style=""fill:none;stroke:#ffffff;stroke-width:16;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""M 43.971765,128 C 184.92235,56.01882 327.07764,56.01882 468.02823,128""
           id=""path10076""
           sodipodi:nodetypes=""cc"" />
        <path
           style=""fill:none;stroke:#ffffff;stroke-width:16.9737;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""m 17.381149,184.06667 c 158.627301,-53.30824 318.610391,-53.30824 477.237701,0""
           id=""path10076-5""
           sodipodi:nodetypes=""cc"" />
      </g>
      <g
         id=""g13286-0""
         transform=""matrix(1,0,0,-1,0,512)"">
        <circle
           style=""fill:none;fill-opacity:1;stroke:#ffffff;stroke-width:15.938;stroke-linecap:square;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1;paint-order:fill markers stroke""
           id=""path7876-4""
           cx=""256""
           cy=""256""
           r=""248.03113"" />
        <path
           style=""fill:none;stroke:#ffffff;stroke-width:16;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""M 8.4329412,256 H 503.56706""
           id=""path10022-2"" />
        <path
           style=""fill:none;stroke:#ffffff;stroke-width:16;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""M 43.971765,128 C 184.92235,56.01882 327.07764,56.01882 468.02823,128""
           id=""path10076-9""
           sodipodi:nodetypes=""cc"" />
        <path
           style=""fill:none;stroke:#ffffff;stroke-width:16.9737;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""m 17.381149,184.06667 c 158.627301,-53.30824 318.610391,-53.30824 477.237701,0""
           id=""path10076-5-8""
           sodipodi:nodetypes=""cc"" />
      </g>
    </g>
    <g
       id=""g13340-2""
       transform=""rotate(90,256,256)"">
      <g
         id=""g13286-7"">
        <circle
           style=""fill:none;fill-opacity:1;stroke:#ffffff;stroke-width:15.938;stroke-linecap:square;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1;paint-order:fill markers stroke""
           id=""path7876-7""
           cx=""256""
           cy=""256""
           r=""248.03113"" />
        <path
           style=""fill:none;stroke:#ffffff;stroke-width:16;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""M 8.4329412,256 H 503.56706""
           id=""path10022-1"" />
        <path
           style=""fill:none;stroke:#ffffff;stroke-width:16;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""M 43.971765,128 C 184.92235,56.01882 327.07764,56.01882 468.02823,128""
           id=""path10076-7""
           sodipodi:nodetypes=""cc"" />
        <path
           style=""fill:none;stroke:#ffffff;stroke-width:16.9737;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""m 17.381149,184.06667 c 158.627301,-53.30824 318.610391,-53.30824 477.237701,0""
           id=""path10076-5-83""
           sodipodi:nodetypes=""cc"" />
      </g>
      <g
         id=""g13286-0-5""
         transform=""matrix(1,0,0,-1,0,512)"">
        <circle
           style=""fill:none;fill-opacity:1;stroke:#ffffff;stroke-width:15.938;stroke-linecap:square;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1;paint-order:fill markers stroke""
           id=""path7876-4-8""
           cx=""256""
           cy=""256""
           r=""248.03113"" />
        <path
           style=""fill:none;stroke:#ffffff;stroke-width:16;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""M 8.4329412,256 H 503.56706""
           id=""path10022-2-5"" />
        <path
           style=""fill:none;stroke:#ffffff;stroke-width:16;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""M 43.971765,128 C 184.92235,56.01882 327.07764,56.01882 468.02823,128""
           id=""path10076-9-1""
           sodipodi:nodetypes=""cc"" />
        <path
           style=""fill:none;stroke:#ffffff;stroke-width:16.9737;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""m 17.381149,184.06667 c 158.627301,-53.30824 318.610391,-53.30824 477.237701,0""
           id=""path10076-5-8-3""
           sodipodi:nodetypes=""cc"" />
      </g>
    </g>
  </g>
</svg>
";
                GenerateSVG("InstalledModPanel/ViewModPage", svg);
            }

            private static void GenerateDeleteSVG() {
                string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Delete.svg""
   xmlns:inkscape=""http://www.inkscape.org/namespaces/inkscape""
   xmlns:sodipodi=""http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd""
   xmlns=""http://www.w3.org/2000/svg""
   xmlns:svg=""http://www.w3.org/2000/svg"">
  <sodipodi:namedview
     id=""namedview7""
     pagecolor=""#505050""
     bordercolor=""#eeeeee""
     borderopacity=""1""
     inkscape:pageshadow=""0""
     inkscape:pageopacity=""0""
     inkscape:pagecheckerboard=""0""
     inkscape:document-units=""px""
     showgrid=""false""
     units=""px""
     showguides=""true""
     inkscape:guide-bbox=""true""
     inkscape:zoom=""0.83007813""
     inkscape:cx=""143.96235""
     inkscape:cy=""366.83294""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""1912""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""434.94509,256""
       orientation=""0,1""
       id=""guide1388""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""256,311.38798""
       orientation=""-1,0""
       id=""guide1390""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""256,319.48061""
       orientation=""0,1""
       id=""guide8062""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""386.10824,128""
       orientation=""0,1""
       id=""guide8086""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""314.42824,673.43059""
       orientation=""0,-1""
       id=""guide9987"" />
    <sodipodi:guide
       position=""440.01782,432.03564""
       orientation=""0,-1""
       id=""guide10173"" />
    <sodipodi:guide
       position=""99.27496,392.5591""
       orientation=""-1,0""
       id=""guide10175""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""324.06588,414.41882""
       orientation=""1,0""
       id=""guide10177"" />
  </sodipodi:namedview>
  <defs
     id=""defs2"" />
  <g
     inkscape:label=""Layer 1""
     inkscape:groupmode=""layer""
     id=""layer1"">
    <rect
       style=""fill:none;fill-opacity:1;stroke:#ffffff;stroke-width:18.384;stroke-linecap:square;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1;paint-order:fill markers stroke""
       id=""rect13584""
       width=""249.45009""
       height=""383.3671""
       x=""131.27496""
       y=""119.4409""
       ry=""0.089272805"" />
    <rect
       style=""fill:none;fill-opacity:1;stroke:#ffffff;stroke-width:16.7024;stroke-linecap:square;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1;paint-order:fill markers stroke""
       id=""rect13688""
       width=""385.0488""
       height=""50.256187""
       x=""63.475605""
       y=""59.992672""
       ry=""0.082174592"" />
    <rect
       style=""fill:none;fill-opacity:1;stroke:#ffffff;stroke-width:13.9318;stroke-linecap:square;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1;paint-order:fill markers stroke""
       id=""rect13688-3""
       width=""253.9023""
       height=""53.026787""
       x=""129.04886""
       y=""6.9658899""
       ry=""0.086704835"" />
    <path
       style=""fill:none;stroke:#ffffff;stroke-width:16.7396;stroke-linecap:round;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
       d=""M 256,177.75078 V 445.58486""
       id=""path13825"" />
    <path
       style=""fill:none;stroke:#ffffff;stroke-width:16.7396;stroke-linecap:round;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
       d=""M 327.21236,177.75078 V 445.58486""
       id=""path13825-7"" />
    <path
       style=""fill:none;stroke:#ffffff;stroke-width:16.7396;stroke-linecap:round;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
       d=""M 178.48567,177.75078 V 445.58486""
       id=""path13825-7-3"" />
  </g>
</svg>
";
                GenerateSVG("InstalledModPanel/Delete", svg);
            }

            private static void GenerateDownloadSVG() {
                string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Download.svg""
   xmlns:inkscape=""http://www.inkscape.org/namespaces/inkscape""
   xmlns:sodipodi=""http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd""
   xmlns=""http://www.w3.org/2000/svg""
   xmlns:svg=""http://www.w3.org/2000/svg"">
  <sodipodi:namedview
     id=""namedview7""
     pagecolor=""#505050""
     bordercolor=""#eeeeee""
     borderopacity=""1""
     inkscape:pageshadow=""0""
     inkscape:pageopacity=""0""
     inkscape:pagecheckerboard=""0""
     inkscape:document-units=""px""
     showgrid=""false""
     units=""px""
     showguides=""true""
     inkscape:guide-bbox=""true""
     inkscape:zoom=""1.1739077""
     inkscape:cx=""126.50057""
     inkscape:cy=""289.20501""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""1912""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""434.94509,331.56314""
       orientation=""0,-1""
       id=""guide1388"" />
    <sodipodi:guide
       position=""256,311.38798""
       orientation=""-1,0""
       id=""guide1390""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
  </sodipodi:namedview>
  <defs
     id=""defs2"" />
  <g
     inkscape:label=""Layer 1""
     inkscape:groupmode=""layer""
     id=""layer1"">
    <g
       id=""g1456""
       transform=""matrix(1,0,0,-1,0,511.98516)"">
      <path
         id=""rect843""
         style=""fill:#ffffff;fill-opacity:1;stroke:none;stroke-width:2.85941;stroke-linecap:square;paint-order:fill markers stroke""
         d=""m 256.0521,1.4393963 178.89297,178.8929737 2e-5,0.10449 -357.890451,-1e-5 v -0.10451 L 255.94761,1.4393715 c 0.0289,-0.028935 0.0755,-0.02897 0.10449,2.12e-5 z""
         sodipodi:nodetypes=""cccccssc"" />
      <rect
         style=""fill:#ffffff;fill-opacity:1;stroke:none;stroke-width:2.87977;stroke-linecap:square;paint-order:fill markers stroke""
         id=""rect1306""
         width=""125.12022""
         height=""330.09476""
         x=""193.4399""
         y=""180.47275""
         ry=""0.076415896"" />
    </g>
  </g>
</svg>
";
                GenerateSVG("OnlineModPanel/Download", svg);
            }
        }
    }
}
