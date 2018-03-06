using ClosedXML.Excel;
using LinqToDB.Data;
using Mrv.Regatta.PreisDesMinisterpraesidenten.Db.DataModels;
using Mrv.Regatta.PreisDesMinisterpraesidenten.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using XmlBase;

namespace Mrv.Regatta.PreisDesMinisterpraesidenten
{
    public class Calculation
    {
        EinstellungenProfil _profil;
        List<ZielZeiten> _zielZeiten;
        List<TRennen> _races;
        List<TBoote> _boats;
        List<TVerein> _clubs;

        /// <summary>
        /// Initializes a new instance of the <see cref="Calculation"/> class.
        /// </summary>
        public Calculation()
        {
            // Einstellungs-Datei öffnen
            var settings = Einstellungen.Load<Einstellungen>(new XmlFilePath("einstellungen.xml"));

            // Name des zu verwendenden Profils aus den Einstellungen
            var profilName = settings.Profilname;
            Tools.OutputText("Verwendetes Profil: " + profilName, ConsoleColor.DarkGray);
            Tools.OutputText();

            // Es interessiert von den Einstellungen nur das geforderte Profil
            _profil = settings.Profil.Single(p => p.Profilname.Equals(profilName, StringComparison.OrdinalIgnoreCase));

            // Connection Strings
            var dbConfigurations = new DbConfigurations();
            dbConfigurations.Add("DatenDb", _profil.ConnectionStringDatenDb.ProviderName, _profil.ConnectionStringDatenDb.ConnectionString);
            dbConfigurations.Add("ZeitmessDb", _profil.ConnectionStringZeitmessDb.ProviderName, _profil.ConnectionStringZeitmessDb.ConnectionString);

            DataConnection.DefaultSettings = dbConfigurations;

            // Datenbanken einlesen

            // DatenDB
            using (var db = new DatenDB("DatenDb"))
            {
                _races = db.TRennens.OrderBy(x => x.RTag).ThenBy(x => x.RZeit).ToList();
                _boats = db.TBootes.ToList();
                _clubs = db.TVereins.ToList();
                Tools.OutputText("Connection-String für 'DatenDB':", ConsoleColor.DarkYellow);
                Tools.OutputText($"({db.ConnectionString})", ConsoleColor.DarkYellow);
                Tools.OutputText(db.ConnectionString, ConsoleColor.Yellow);
                Tools.OutputText();
            }

            // ZeitmessDB
            using (var db = new ZeitmessDB("ZeitmessDb"))
            {
                _zielZeiten = db.ZielZeitens.ToList();
                Tools.OutputText("Connection-String für 'ZeitmessDB':", ConsoleColor.DarkYellow);
                Tools.OutputText($"({db.ConnectionString})", ConsoleColor.DarkYellow);
                Tools.OutputText(db.ConnectionString, ConsoleColor.Yellow);
                Tools.OutputText();
            }

            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // !!! manuelle Korrekturen !!!
            /*
            var race = _races.SingleOrDefault(r => r.RNr == "LE255" && r.RLaufTyp == "EF"));
            if (race != null)
            {
                race.RAnzRudererBoot = 4;
            }
            */
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }

        /// <summary>
        /// Runs the calculation.
        /// </summary>
        public void Run()
        {
            var tableRaces = new List<Race>();

            // Reguläre Ausdrücke für Rennen und Lauftyp (EF, VL, ...)
            var regExRace = new Regex(_profil.RegExRennen);
            var regExLaufTyp = new Regex(_profil.RegExLaufTyp);

            #region  alle Rennen durchgehen
            
            foreach (var race in _races)
            {
                // Renn-Nr. und -Zeit ausgeben
                Tools.OutputText(race.RNr, false);
                Tools.OutputText($" ({race.RTag}, {((DateTime)race.RZeit).ToString("HH:mm")} - {race.RLaufTyp})", ConsoleColor.DarkGray, false);
                Tools.OutputText(":", false);

                // nur bestimmte Rennen, Renn-Nr. gegen RegEx prüfen
                if (regExRace.IsMatch(race.RNr))
                {
                    // schauen, ob es Boote zu dem Rennen gibt
                    if (_boats.Any(x => x.BRNr == race.Index))
                    {
                        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        // zum Debuggen
                        if (race.RNr == "K418")
                        {
                            // zum Debuggen
                        }
                        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                        // Zielzeiten für für die aktuelle Renn-Nummer
                        var zeiten = _zielZeiten.Where(z => z.RennNr == race.RNr).ToList();

                        // diese Zeiten enthalten jetzt aber alle Zeiten dieser Renn-Nr., also zum Beispiel auch die Zeiten der Vorläufe

                        // nur "EF" zählt, Rest (z. B. Vorläufe) uninteressant, mit RegEx filtern
                        zeiten = zeiten.Where(x => regExLaufTyp.IsMatch(x.LaufTyp)).ToList();

                        if (zeiten.Any())
                        {
                            // jetzt ist noch eine Unterscheidung der Abteilung erforderlich
                            var abteilungen = zeiten.GroupBy(x => x.AbtNr).OrderBy(x => x.Key);

                            // Anzahl Ruderer bestimmen
                            var rowersCount = (int)race.RAnzRudererBoot;

                            // gesteuerten Vierer korrigieren
                            if (rowersCount == 5)
                            {
                                rowersCount = 4;
                            }

                            // gesteuerten Achten korrigieren
                            if (rowersCount == 9)
                            {
                                rowersCount = 8;
                            }

                            // Neuer Eintrag (Spalte) für die Tabelle
                            var tableRace = new Race()
                            {
                                Name = race.RNr,
                                RowersCount = rowersCount
                            };

                            // neuen Eintrag hinzufügen
                            tableRaces.Add(tableRace);

                            foreach (var abteilung in abteilungen)
                            {
                                // Nr. der Abteilung
                                var abtName = abteilung.Key;

                                // alle Zeiten der Abteilung
                                zeiten = abteilung.Select(x => x).ToList();

                                // Abteilung ausgeben
                                Tools.OutputText(" ", false);
                                Tools.OutputText($"[{abtName}]", ConsoleColor.DarkGreen, false);

                                // Zeiten mit "Bemerkung = 8" ("zeitgleich"): Warnen
                                if (zeiten.Any(z => z.Bem == 8))
                                {
                                    Tools.OutputText(" ", false);
                                    Tools.OutputText("Bem=8 ('zeitgleich')", ConsoleColor.White, ConsoleColor.DarkCyan, false);
                                }

                                // gekennterte, disqualifizierte, ... Boote (mit "Bemerkung: 0-7" ("zeitgleich"): Warnen
                                if (zeiten.Any(z => z.Bem >= 1 && z.Bem <= 7))
                                {
                                    Tools.OutputText(" ", false);
                                    Tools.OutputText("Mindestens eine Zielzeit ungülig (disqualifiziert/gekentert/...)", ConsoleColor.Cyan, ConsoleColor.Blue, false);

                                    // nur Bemerkung 0 oder 8 behalten
                                    zeiten = zeiten.Where(z => z.Bem == 0 || z.Bem == 8).ToList();
                                }

                                // wenn nach der vielen Filterei jetzt noch Zeiten übrig sind, kann man jetzt loslegen
                                if (zeiten.Any())
                                {
                                    // Zeiten nach Uhrzeit sortieren, damit wandert die "Start-Zeit" ganz nach vorne
                                    zeiten = zeiten.OrderBy(x => x.Zeit).ToList();

                                    #region Plausibilität prüfen

                                    if (!zeiten.First().Start.ToString().ToUpper().Equals("x", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // erstes Element ist nicht Start-Zeit (X)
                                        Tools.OutputText(" ", false);
                                        Tools.OutputText("Erstes Element ist nicht Start-Zeit (X)", ConsoleColor.Yellow, ConsoleColor.Red, false);
                                    }

                                    if (zeiten.Count(x => x.Start.ToString().ToUpper().Equals("x", StringComparison.OrdinalIgnoreCase)) > 1)
                                    {
                                        // mehr als 1 "Start" gefunden
                                        Tools.OutputText(" ", false);
                                        Tools.OutputText("Mehr als 1 'Start' gefunden", ConsoleColor.Yellow, ConsoleColor.Red, false);
                                    }

                                    if (zeiten.Count() == 1)
                                    {
                                        // es gibt nur "Start" und sonst nichts
                                        Tools.OutputText(" ", false);
                                        Tools.OutputText("Es gibt nur 'Start' und sonst nichts", ConsoleColor.Yellow, ConsoleColor.Red, false);
                                    }

                                    if (race.RAnzRudererBoot == null)
                                    {
                                        // keine vernünftige Anzahl Ruderer (race.RAnzRudererBoot = NULL)
                                        Tools.OutputText(" ", false);
                                        Tools.OutputText("Keine vernünftige Anzahl Ruderer (race.RAnzRudererBoot = NULL)", ConsoleColor.Yellow, ConsoleColor.Red, false);
                                        race.RAnzRudererBoot = 0; // weil es unten abgefragt wird, dann sollte es nicht NULL sein 
                                    }
                                    else
                                    {
                                        if (race.RAnzRudererBoot == 0)
                                        {
                                            // keine vernünftige Anzahl Ruderer (race.RAnzRudererBoot = 0)
                                            Tools.OutputText(" ", false);
                                            Tools.OutputText("Keine vernünftige Anzahl Ruderer (TRennen.RAnzRudererBoot = 0)", ConsoleColor.Yellow, ConsoleColor.Red, false);
                                        }
                                    }

                                    #endregion Plausibilität prüfen

                                    // Start-Zeit aus der Liste entfernen
                                    zeiten.RemoveAt(0);

                                    // Die einzelnen Plätze durchgehen
                                    SetPoints(zeiten, 1, rowersCount, race, tableRace);
                                    SetPoints(zeiten, 2, rowersCount, race, tableRace);
                                    SetPoints(zeiten, 3, rowersCount, race, tableRace);
                                    SetPoints(zeiten, 4, rowersCount, race, tableRace);
                                    SetPoints(zeiten, 5, rowersCount, race, tableRace);
                                    SetPoints(zeiten, 6, rowersCount, race, tableRace);

                                    // Fertig
                                    Tools.OutputText(" ", false);
                                    Tools.OutputText("OK", ConsoleColor.Green, false);
                                }
                                else
                                {
                                    // keine Zielzeiten 
                                    Tools.OutputText(" ", false);
                                    Tools.OutputText("Keine Zielzeiten (2)", ConsoleColor.Yellow, false);
                                }
                            }
                        }
                        else
                        {
                            // keine Zielzeiten 
                            Tools.OutputText(" ", false);
                            Tools.OutputText("Keine Zielzeiten (1)", ConsoleColor.Yellow, false);
                        }
                        
                    }
                    else
                    {
                        // keine Boote
                        Tools.OutputText(" ", false);
                        Tools.OutputText("Keine Boote zu diesem Rennen", ConsoleColor.Cyan, false);
                    }
                }
                else
                {
                    // regulärer Ausdruck passt nicht
                    Tools.OutputText(" ", false);
                    Tools.OutputText("Renn-Nr. entspricht nicht der Vorgabe", ConsoleColor.Magenta, false);
                }

                // nächste Zeile
                Tools.OutputText();
            }

            #endregion

            // Tabelle nun noch ausgeben

            // es wird eine Liste aller Vereine benötigt, die Punkte erreicht haben (egal in welchem Rennen)
            var clubList = new List<Club>();

            // Rennen der Tabelle durchgehen
            foreach (var tableRace in tableRaces)
            {
                // Vereine mit Punkten durchgehen
                foreach(var club in tableRace.Clubs)
                {
                    if (!clubList.Any(c => c.Verein.VIDVerein == club.Verein.VIDVerein))
                    {
                        clubList.Add(club);
                    }
                }
            }

            // sortieren (es soll nach "VStadt" sortiert werden)
            clubList = clubList.OrderBy(x => x.Verein.VStadt).ToList();

            #region CSV-Datei erzeugen
            
            using (var file = new System.IO.StreamWriter(@"results.csv"))
            {
                string line;

                // Header
                file.WriteLine("Verein;" + string.Join(";", tableRaces.Select(r => r.Name)) + ";Summe");

                // Anzahl Ruderer
                file.WriteLine(";" + string.Join(";", tableRaces.Select(r => r.RowersCount.ToString())));

                // alle Vereine die in der Tabelle vorkommen durchgehen
                foreach (var club in clubList)
                {
                    // für jeden Verein die Rennen durchgehen
                    float count = 0;
                    line = club.Verein.VVereinsnamenKurz + ";";
                    foreach (var tableRace in tableRaces)
                    {
                        // schauen, wie viel Punkte der aktuelle Verein in aktuellen Rennen gemacht hat
                        var club1 = tableRace.Clubs.SingleOrDefault(x => x.Verein.VIDVerein == club.Verein.VIDVerein);
                        if (club1 == null)
                        {
                            // Verein hat gar keine Punkte gemacht, nichts einzutragen
                            line += ";";
                        }
                        else
                        {
                            // Verein hat Punkte gemacht
                            line += club1.Points.ToString() + ";";
                            count += club1.Points;
                        }
                    }

                    // (horizontale) Summe hinten noch dranhängen
                    line += count.ToString();
                    file.WriteLine(line);
                }

                // (vertikale) Summe

                // für jedes Rennen die Sumem bilden
                
                // alle Rennen durchgehen
                line = ";";
                foreach (var tableRace in tableRaces)
                {
                    // alle Vereine, die in diesem Rennen Punkte gemacht haben, durchgehen und aufsummieren
                    float count = 0;
                    foreach (var club1 in tableRace.Clubs)
                    {
                        count += club1.Points;
                    }
                    line += count.ToString() + ";";
                }
                file.WriteLine(line);
            }

            Tools.OutputText();
            Tools.OutputText("CSV-Datei erzeugt.", ConsoleColor.DarkYellow);

            #endregion

            #region XLSX-Datei erzeugen

            var clubsRowStart = 3;
            var racesColStart = 2;
            var CELLSUMME = "Summe";

            var xlsxFile = "results.xlsx";
            if (!System.IO.File.Exists(xlsxFile))
            {
                // Datei existiert noch nicht
                
                var racesColEnd = 2 + tableRaces.Count() - 1; // das hier darf man nicht verwenden, wenn die Datei schon existiert schließilch könnten in der Datei weniger Einträge drin stehen!
                var clubsRowEnd = 3 + clubList.Count() - 1;   // das hier darf man nicht verwenden, wenn die Datei schon existiert schließilch könnten in der Datei weniger Einträge drin stehen!

                #region Datei neu erzeugen

                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Tabelle");

                // ------------------------------------- Header

                var row = 1;
                worksheet.Cell(row, 1).Value = "Verein";

                var col = racesColStart;
                foreach (var tableRace in tableRaces)
                {
                    worksheet.Cell(row, col).Value = tableRace.Name;
                    worksheet.Cell(row, col).Style.Font.Bold = true;
                    col++;
                }

                worksheet.Cell(row, racesColEnd + 1).Value = CELLSUMME;
                worksheet.Cell(row, racesColEnd + 2).Value = "Korrektur";
                worksheet.Cell(row, racesColEnd + 3).Value = "Gesamtsumme";

                // ------------------------------------- Anzahl Ruderer
                row = 2;
                col = racesColStart;
                foreach (var tableRace in tableRaces)
                {
                    worksheet.Cell(row, col).Value = tableRace.RowersCount;
                    worksheet.Cell(row, col).Style.Font.Italic = true;
                    col++;
                }

                // ------------------------------------- Vereine
                row = 3;

                // alle Vereine die in der Tabelle vorkommen durchgehen ---> Zeilen
                foreach (var club in clubList)
                {
                    // für jeden Verein die Rennen durchgehen
                    float count = 0;

                    // Vereinsname
                    worksheet.Cell(row, 1).Value = club.Verein.VVereinsnamenKurz;
                    worksheet.Cell(row, 1).Style.Font.Bold = true;

                    col = racesColStart;
                    foreach (var tableRace in tableRaces) // ---> Spalten
                    {
                        // schauen, wie viel Punkte der aktuelle Verein in aktuellen Rennen gemacht hat
                        var club1 = tableRace.Clubs.SingleOrDefault(x => x.Verein.VIDVerein == club.Verein.VIDVerein);
                        if (club1 == null)
                        {
                            // Verein hat gar keine Punkte gemacht, nichts einzutragen
                        }
                        else
                        {
                            // Verein hat Punkte gemacht
                            worksheet.Cell(row, col).Value = club1.Points;
                        }

                        col++;
                    }

                    // (horizontale) Summe hinten noch dranhängen
                    worksheet.Cell(row, col).FormulaA1 = $"sum({racesColStart.Col()}{row}:{racesColEnd.Col()}{row})";
                    worksheet.Cell(row, col).Style.Font.Italic = true;

                    // Summe mit Korrektur noch dranhängen (1 Spalte dazwischen frei
                    worksheet.Cell(row, col + 2).FormulaR1C1 = $"sum({(racesColEnd + 1).Col()}{row}:{(racesColEnd + 2).Col()}{row})";

                    row++;
                }

                // unten dran die vertikale Summe
                worksheet.Cell(row, 1).Value = CELLSUMME;
                col = racesColStart;

                foreach (var tableRace in tableRaces) // ---> Spalten
                {
                    worksheet.Cell(row, col).FormulaA1 = $"sum({col.Col()}{clubsRowStart}:{col.Col()}{clubsRowEnd})";
                    worksheet.Cell(row, col).Style.Font.Italic = true;
                    col++;
                }

                workbook.SaveAs(xlsxFile);

                Tools.OutputText();
                Tools.OutputText("XLSX-Datei erzeugt.", ConsoleColor.DarkYellow);

                #endregion

            }
            else
            {
                // Es gibt die Datei bereits

                #region Datei korrigieren

                var workbook = new XLWorkbook(xlsxFile);

                if (workbook.Worksheets.Count != 1)
                {
                    throw new Exception($"Excel-Datei {xlsxFile} muss genau ein Arbeitsblatt enthalten!");
                }

                // Arbeitsblatt holen
                var worksheet = workbook.Worksheets.Single();

                // Alle Zellen auf weiß
                var cells = worksheet.Cells(false);
                cells.Style.Fill.BackgroundColor = XLColor.NoColor;

                #region Schauen ob in der Excel-Datei eine Zeile (=Verein) fehlt

                // erste Spalte, da stehen die Vereine
                var clubColumn = worksheet.Column(1);

                var clubListToCheck = clubList.ToList(); // Kopie der Liste, die man verändern kann

                // Alle Vereine, die in die Datei müssen durchgehen
                while (clubListToCheck.Any())
                {
                    // Ein Element aus der Liste entfernen und dann betrachtet
                    var clubToCheck = clubListToCheck.First();
                    clubListToCheck.Remove(clubToCheck);

                    // Datei auslesen
                    var sumCell = clubColumn.Cells().First(x => x.GetValue<string>() == CELLSUMME);
                    var clubCells = clubColumn.Cells(3, sumCell.Address.RowNumber - 1);

                    // zu jedem Verein braucht man zum Sortieren nun auch noch die Stadt ("VStadt"), daher das ganze Club-Objekt übernehmen
                    var clubsFromFile = new List<TVerein>();
                    foreach (var clubCell in clubCells)
                    {
                        var vereinsName = clubCell.GetValue<string>();
                        var club = clubList.Single(c => c.Verein.VVereinsnamenKurz == vereinsName);
                        clubsFromFile.Add(club.Verein);
                    }
                    
                    // schauen, ob der Verein schon in der Datei steht
                    var testClub = clubsFromFile.SingleOrDefault(x => x.VVereinsnamenKurz == clubToCheck.Verein.VVereinsnamenKurz);
                    if (testClub != null)
                    {
                        // Verein gefunden
                        // alles gut
                    }
                    else
                    {
                        // Verein nicht gefunden, das ist schlecht
                        // => dann muss er hinzugefügt werden

                        // Um zu sehen wo er in der Datei eingefügt werden muss, wird er an die Clubliste hinten dran gefügt.
                        // Die Liste wird dann sortiert, dann wird geschaut wo das Element nach dem Sortieren gelandet ist und
                        // damit hat man den Index für das Einsortieren in die Datei.
                        // Sortierung ist anhand der Stadt gewünscht!

                        clubsFromFile.Add(clubToCheck.Verein);
                        clubsFromFile.Sort((x, y) => x.VStadt.CompareTo(y.VStadt));

                        var index = clubsFromFile.IndexOf(clubToCheck.Verein);

                        var rowNumber = clubsRowStart + index;
                        worksheet.Row(rowNumber).InsertRowsAbove(1);
                        var newCell = worksheet.Cell(rowNumber, 1);
                        newCell.SetValue<string>(clubToCheck.Verein.VVereinsnamenKurz);
                        newCell.Style.Fill.BackgroundColor = XLColor.Red;

                        // Summen-Spalten (hinten) erstellen
                        var cellSumme = worksheet.Row(1).Cells().Single(x => x.GetValue<string>() == CELLSUMME);
                        var colCellSumme = cellSumme.Address.ColumnNumber;
                    }
                }

                #endregion

                #region Schauen ob in der Excel-Datei eine Spalte (=Rennen) fehlt

                {
                    // erste Spalte, da stehen die Vereine
                    var racesRow = worksheet.Row(1);

                    var races = tableRaces.ToList(); // Kopie der Liste, die man verändern kann

                    while (races.Any())
                    {
                        // Ein Element aus der Liste entfernen und dann betrachtet
                        var raceToCheck = races.First();
                        races.Remove(raceToCheck);

                        // schauen, ob das Rennen schon in der Datei steht
                        var testRace = racesRow.Cells().SingleOrDefault(x => x.GetValue<string>() == raceToCheck.Name);
                        if (testRace != null)
                        {
                            // es gibt das Rennen schon in der Datei
                            // => alles OK
                        }
                        else
                        {
                            // es gibt das Rennen onch nicht in der Datei
                            // schlecht

                            int columnNumberNewRace;

                            // wenn es dieses Rennen in der Datei nicht gibt, dann muss es aber zumindest das Rennen davon geben!
                            // nach dem Rennen in der Original-Liste suchen
                            var indexCurrentRace = tableRaces.IndexOf(raceToCheck);
                            if (indexCurrentRace == 0)
                            {
                                // es ist das 1. Rennen, dann kann es kein Rennen "davor" geben
                                columnNumberNewRace = 2;
                            }
                            else
                            {
                                // es ist nicht das 1. Rennen, dann müsste es ein Rennen davor geben
                                var raceBeforeIndex = indexCurrentRace - 1;
                                var raceBefore = tableRaces[raceBeforeIndex];

                                // nach diesem Rennen nun in der Datei suchen
                                var testRaceBefore = racesRow.Cells().SingleOrDefault(x => x.GetValue<string>() == raceBefore.Name);

                                if (testRaceBefore == null)
                                {
                                    // auch das Rennen davor wird nicht gefunden
                                    // => darf nicht sein
                                    throw new Exception($"Rennen '{raceToCheck.Name}' nicht in Datei gefunden und das Rennen davor ('{raceBefore.Name}') auch nicht!");
                                }

                                columnNumberNewRace = testRaceBefore.Address.ColumnNumber + 1;
                            }

                            worksheet.Column(columnNumberNewRace).InsertColumnsBefore(1);

                            // Name Rennen
                            var cell1 = worksheet.Cell(1, columnNumberNewRace);
                            cell1.SetValue<string>(raceToCheck.Name);
                            cell1.Style.Fill.BackgroundColor = XLColor.Red;

                            // Anzahl Ruderer
                            var cell2 = worksheet.Cell(2, columnNumberNewRace);
                            cell2.SetValue<int>(raceToCheck.RowersCount);
                            cell2.Style.Font.Italic = true;
                            cell2.Style.Fill.BackgroundColor = XLColor.Red;
                        }
                    }

                }

                #endregion

                #region Alle Summen-Formeln (unten) anpassen

                {
                    var cellSummeUnten = worksheet.Column(1).Cells().First(x => x.GetValue<string>() == CELLSUMME);
                    var rowNumberSummeUnten = cellSummeUnten.Address.RowNumber;
                    var cellSummeRechts = worksheet.Row(1).Cells().First(x => x.GetValue<string>() == CELLSUMME);
                    var columnNumberSummeRechts = cellSummeRechts.Address.ColumnNumber;

                    for (int columnNo = 2; columnNo <= (columnNumberSummeRechts - 1); columnNo++)
                    {
                        var cellSumme = worksheet.Cell(rowNumberSummeUnten, columnNo);
                        cellSumme.FormulaA1 = $"sum({cellSumme.Address.ColumnNumber.Col()}{clubsRowStart}:{cellSumme.Address.ColumnNumber.Col()}{cellSumme.Address.RowNumber - 1})";
                        cellSumme.Style.Font.Italic = true;
                    }
                }

                #endregion

                #region Alle Summen-Formeln (hinten) anpassen

                {
                    var cellSummeUnten = worksheet.Column(1).Cells().First(x => x.GetValue<string>() == CELLSUMME);
                    var rowNumberSummeUnten = cellSummeUnten.Address.RowNumber;
                    var cellSummeRechts = worksheet.Row(1).Cells().First(x => x.GetValue<string>() == CELLSUMME);
                    var columnNumberSummeRechts = cellSummeRechts.Address.ColumnNumber;

                    for (int rowNo = 3; rowNo <= (rowNumberSummeUnten - 1); rowNo++)
                    {
                        // 1. Summme
                        var cellSumme = worksheet.Cell(rowNo, columnNumberSummeRechts);
                        cellSumme.FormulaA1 = $"sum({racesColStart.Col()}{rowNo}:{(cellSumme.Address.ColumnNumber - 1).Col()}{rowNo})";
                        cellSumme.Style.Font.Italic = true;

                        // 2. Summme, zwei Spalten weiter
                        cellSumme = worksheet.Cell(rowNo, columnNumberSummeRechts + 2);
                        cellSumme.FormulaA1 = $"sum({cellSummeRechts.Address.ColumnNumber.Col()}{rowNo}:{(cellSummeRechts.Address.ColumnNumber + 1).Col()}{rowNo})";
                        cellSumme.Style.Font.Italic = true;
                    }
                }

                #endregion

                #region Alle Daten durchgehen, schauen, ob sie schon drin stehen

                // alle Rennen durchgehen
                foreach (var race in tableRaces)
                {
                    // das Rennen finden
                    var raceCell = worksheet.Row(1).Cells().Single(x => x.GetValue<string>() == race.Name);

                    foreach (var club in race.Clubs)
                    {
                        // den Verein finden
                        var clubCell = worksheet.Column(1).Cells().Single(x => x.GetValue<string>() == club.Verein.VVereinsnamenKurz);

                        var cell = worksheet.Cell(clubCell.Address.RowNumber, raceCell.Address.ColumnNumber);

                        if (cell.GetValue<string>() != club.Points.ToString())
                        {
                            cell.Value = club.Points;
                            cell.Style.Fill.BackgroundColor = XLColor.Red;
                        }
                    }
                }

                #endregion

                workbook.Save();

                Tools.OutputText();
                Tools.OutputText("XLSX-Datei angepasst.", ConsoleColor.DarkYellow);

                #endregion

            }

            #endregion
        }

        /// <summary>
        /// Sets the points.
        /// </summary>
        /// <param name="zielzeiten">The zielzeiten.</param>
        /// <param name="platz">The platz.</param>
        /// <param name="rowersCount">The rowers count.</param>
        private void SetPoints(List<ZielZeiten> zielzeiten, int platz, int rowersCount, TRennen race, Race tableRace)
        {
            // es sind noch Zielzeiten übrig, die noch nicht verarbeitet wurden
            if (zielzeiten.Any())
            {
                // 1.) Zuerst Startnummer bestimmen...
                var startNummer = zielzeiten.First().StartNr;
                var raceNumber = zielzeiten.First().RennNr;

                // Die Rennnummer der Zielzeit muss gleich der Rennnummer sein, die gerade behandel wird, sonst stimmt irgendwas überhaupt nicht
                if (raceNumber != race.RNr)
                {
                    throw new Exception("Rennnummern passen nicht!");
                }

                // 2.) ...und dann das Element verwerfen (damit es beim nächsten Mal nicht nochmal dran kommt)
                zielzeiten.RemoveAt(0);

                // Punkte zum Platz ermitteln
                var points = GetPoints(platz, rowersCount);

                // Verein bestimmen, Umweg über das Boot nötig
                var boat = _boats.Single(b => (b.BSNr == startNummer) && (b.BRNr == race.Index));
                var club = _clubs.Single(c => c.VIDVerein == boat.BVID);

                // Untervereine bestimmen (Renngemenischaften!)
                // wenn es keine Renngemeinschaften gibt, dann steht hier nur der Verein selbst nochmal drin
                var subClubsString = club.VUnterverein;
                var subClubsTest = subClubsString.Split(';');

                // schauen wie viele Untervereine es gibt, falls > 1, dann ist es eine Renngemeinschaft
                if (subClubsTest.Count() > 1)
                {
                    // Renngemeinschaft
                    Tools.OutputText(" ", false);
                    Tools.OutputText("RGM", ConsoleColor.White, ConsoleColor.DarkGreen, false);
                }

                string[] subClubs;
                switch (_profil.RenngemeinschaftenTeilen)
                {
                    case EinstellungenProfilRenngemeinschaftenTeilen.NichtTeilen:
                        // keine Teilung
                        // die Renngemeinschaft bekommt die Punkte alleine, die beteiligten Vereine gehen leer aus.
                        // die Liste der Renngemeinschaften besteht nur aus dem Verein (der Renngemeinschaft) selbst
                        subClubs = new string[] { club.VIDVerein.ToString() };
                        break;

                    case EinstellungenProfilRenngemeinschaftenTeilen.TeilenMitPunkte:
                        // Renngemeinschaft teilen
                        subClubs = subClubsString.Split(';');

                        // Punkte gleich auch noch teilen durch die Anzahl der Vereine der Renngemeinschaft
                        points /= subClubs.Count();

                        break;

                    case EinstellungenProfilRenngemeinschaftenTeilen.TeilenOhnePunkte:
                        // Renngemeinschaft teilen
                        subClubs = subClubsString.Split(';');

                        // (Punkte bleiben wie sie sind)

                        break;

                    default:
                        throw new ArgumentException("Unbekannter Wert für 'RenngemeinschaftenTeilen'!");
                }

                // Untervereine durchgehen
                foreach (var subClub in subClubs)
                {
                    // Unterverein bestimmen
                    var subClubId = Convert.ToInt32(subClub.Trim());
                    var club1 = _clubs.Single(c => c.VIDVerein == subClubId);

                    // Unterverein bekommt die Punkte

                    // schauen, ob es den Verein schon in der Liste gibt
                    var tableClub = tableRace.Clubs.SingleOrDefault(c => c.Verein.VIDVerein == club1.VIDVerein);
                    if (tableClub == null)
                    {
                        // es gibt den Verein noch nicht in der Liste
                        // => neu anlegen
                        tableRace.Clubs.Add(new Club() { Verein = club1, Points = points });
                    }
                    else
                    {
                        // Verein ist schon in der Liste drin
                        // Punkte nur dazu zählen
                        tableClub.Points += points;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the points.
        /// </summary>
        /// <param name="platz">The platz.</param>
        /// <param name="rowersCount">The rowers count.</param>
        private float GetPoints(int platz, int rowersCount)
        {
            // anhand des gewählten Profils die Punkte ermitteln

            // Bootstyp ermitteln
            CT_Punkte points;
            switch(rowersCount)
            {
                case 1:
                    points = _profil.Wertungsschluessel.Einer;
                    break;

                case 2:
                    points = _profil.Wertungsschluessel.Zweier;
                    break;

                case 4:
                    points = _profil.Wertungsschluessel.Vierer;
                    break;

                case 8:
                    points = _profil.Wertungsschluessel.Achter;
                    break;

                default:
                    // Anzahl Ruderer im Boot ist 0! Das darf nicht sein (Fehlermeldung hierzu
                    // wird ganz am Anfang der Behandlung des aktuellen rennens schon ausgegeben)
                    return 0;
            }

            // Punkte zum Bootstyp ermitteln, abhängig vom Platz
            switch (platz)
            {
                case 1:
                    return points.Platz1;

                case 2:
                    return points.Platz2;

                case 3:
                    return points.Platz3;

                case 4:
                    return points.Platz4;

                case 5:
                    return points.Platz5;

                case 6:
                    return points.Platz6;

                default:
                    return 0;
            }
        }

    }
}
