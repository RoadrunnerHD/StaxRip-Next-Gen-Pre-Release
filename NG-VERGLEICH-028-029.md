# Vergleich 0.28 → 0.29 und Korrekturen für 0.30

Die beiden vollständigen vierteiligen Programmarchive wurden zusammengesetzt und mit dem Archiv-CRC-Test geprüft (beide ohne Archivfehler). Jeder Dateieintrag ist in `Docs/Comparison/dateien-028-029.csv` mit Größe, CRC32 und Änderungszeit aufgeführt. Die 0.29-Archivdatei ist technisch lesbar, enthält jedoch fehlerhafte Programminhalte.

| Dateivergleich | 0.28 | 0.29 |
| --- | ---: | ---: |
| Dateien | 3.197 | 3.199 |
| Apps/Conf | 146 | 146 |
| Apps/Encoders | 78 | 79 |
| Apps/FrameServer | 1.489 | 1.489 |
| Apps/Support | 1.106 | 1.106 |
| Archivgröße | 1.063.279.605 Byte | 959.906.122 Byte |

3.196 Dateien haben denselben Pfad in beiden Archiven. Davon sind 3.193 inhaltlich identisch (CRC32). Nur `StaxRipNG.exe`, `FrameServer.dll` und `Apps/Support/mpv.net/mpvnet.exe` haben einen abweichenden Inhalt. Ausschließlich in 0.28 ist `StaxRip.pdb`; ausschließlich in 0.29 sind `Microsoft.Management.Infrastructure.dll`, `System.Management.Automation.dll` und die temporäre Datei `Apps/Encoders/NVEncC/.nvinfer_10.dll.wqThhv`. Letztere ist 308.281.344 Byte groß. `mpvnet.exe` ist in 0.28 166.362.041 Byte groß, in 0.29 **0 Byte**. Bei 3.190 Dateien mit identischem Inhalt wurde die Änderungszeit verändert; nur drei gemeinsame Dateien behielten ihre Zeit.

## Gesicherte Befunde

| Bereich | Referenz 0.28 | Teststand 0.29 | Änderung im 0.30-Entwurf |
| --- | --- | --- | --- |
| Apps-Anzeige | `Package.Add` ruft `NextGenKeepPackage` auf. Diese Funktion schließt Software-Encoder aus und lässt nur Dateinamen aus einer festen Liste mit 602 Namen zu. | Die Filterfunktion fehlte; alle registrierten StaxRip-Pakete wurden in die Liste aufgenommen, darunter Plugins, die 0.28 nicht anzeigte. | Die Filterbedingung und die Dateinamenliste aus der 0.28-EXE wurden übernommen. |
| App-Status | 146 Konfigurationsdateien unter `Apps/Conf` speichern Version und Datum. | Bei 3.190 unveränderten Dateien wurde die Änderungszeit verändert. `Package.GetStatusVersion` vergleicht diese Zeiten mit den Konfigurationen; Abweichungen über zwei Tage erzeugen einen Versionshinweis. | Die künstlichen FFmpeg-Versionswerte im Quellcode sind entfernt. Beim künftigen Paket müssen die ursprünglichen Änderungszeiten der Apps erhalten und geprüft werden. |
| Inhalt des Testpakets | Das 0.28-Archiv enthält 3.197 Dateien. | Eine temporäre `nvinfer_10.dll`-Datei gelangte ins Paket; `mpvnet.exe` ist als leere Datei vorhanden; zwei zusätzliche PowerShell-Assemblies liegen im Programmordner. | Die vier 0.29-Testteile werden nicht als Release verwendet. Ein neues Paket muss Größe, CRC und Dateiliste mit 0.28 vergleichen. |
| Help-Menü | Einträge rufen `OpenHelpTopic` auf; die Autorenanzeige nennt Roadrunner, Dendraspis und stax76 in der 0.28-Form. | Mehrere Einträge führten direkt zu offiziellen StaxRip-Webseiten. Die Autorenanzeige wich ebenfalls ab. | Help-Verknüpfungen und Autorenanzeige wurden auf die 0.28-Zuordnung gesetzt. „What's new“ öffnet absichtlich den englischen NG-Changelog. |
| „What's new“ | 0.28 ist die inhaltliche Referenz für den Funktionsumfang. | Der hinzugefügte NG-Changelog war auf Deutsch. | Der NG-Changelog ist auf Englisch; Encoder-Buildnummern, Taskleisten-Icon-Satz und gestrichene Oberflächenpunkte stehen nicht darin. |
| EXE-Icon-Ressourcen | 13 Icon-Bilder und eine Icon-Gruppe. | Alle 14 Ressourcen sind bytegleich mit 0.28. | Kein Icon-Bild wurde ausgetauscht. Das gemeldete Taskleistenproblem muss am laufenden Windows-Programm geprüft werden. |

## Weitere Unterschiede im Binärvergleich

Die 0.28-EXE enthält 816 .NET-Typen und 12.614 Methoden; die 0.29-EXE enthält 812 Typen und 12.544 Methoden. Die 0.28-EXE enthält Typen für eine Regex-Suche (`RegexButton`, `RegexHelp`) sowie zusätzliche Methoden für Suchfelder und einige UI-Steuerelemente. Diese fehlen im 0.29-Build. Der 0.29-Quellcode enthält teilweise anders aufgebaute Hardware-Funktionen; aus abweichenden Methodennamen allein folgt noch nicht, dass die Funktion fehlt. Ein Binärvergleich beweist nicht die Gleichwertigkeit jeder UI-Funktion. Diese Punkte sind **noch nicht vollständig portiert oder unter Windows geprüft**.

Weitere nur in 0.28 nachgewiesene Methodennamen sind `NextGenEnsureHardwareEncoder`, `NextGenHardwareProfiles`, `NextGenIsSoftwareEncoder` (`VideoEncoder`), `NextGenFilterVppArgs` (`EncoderParams`), `NextGenAddHardwareControls` und `NextGenRemoveOldWorkflowFiles` (`Folder`), `NextGenCompleteChangelogKey` und `NextGenEnsureMainAudioProfiles` (`MainForm`). Außerdem unterscheiden sich `ButtonEx`, `TextBoxEx`, `TextEdit`, `DataGridViewEx` und `ListViewEx`. Ein Teil der Hardware-Funktionen ist im 0.29-Quellcode bereits anders umgesetzt: `GetDefaults`, `IsAvailable`, die Projektinitialisierung und das Laden von Profilen beschränken Encoder auf NVEnc, VCEEnc, QSVEnc und NullEncoder; QSVEnc verarbeitet `# NGVPP --vpp-...` bereits im Befehlsaufbau. Die in 0.28 vorhandenen Hardware-Crop- und Resize-Einträge in neuen Workflow-Vorlagen wurden für 0.30 ergänzt. Die Regex-Suchoberfläche und weitere UI-Details sind noch nicht portiert. Die komplette Dateiprüfung kann diese Unterschiede sichtbar machen, aber den fehlenden 0.28-Originalquellcode nicht ersetzen. Ein 0.30-Build des vorliegenden Entwurfs ist deshalb zunächst **ein Testbuild**, keine als 0.28-funktionsgleich bestätigte Version.

Die 0.29-Build-ZIP enthält 14 Builddateien und eine EXE mit Version 0.29.0.0. Ein erfolgreicher GitHub-Actions-Build bestätigt nur das Kompilieren. Für 0.30 fehlen bisher ein Windows-Build und ein Programmtest. Für einen vollständig gleichwertigen Quellcode zu 0.28 wäre deren Originalquellcode nötig; die 0.28-EXE allein reicht dafür nicht aus.
