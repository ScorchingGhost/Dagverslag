# LucasBullshit

## Dagverslag downloaden en starten

Open [Releases](https://github.com/ScorchingGhost/Dagverslag/releases/latest) en
download onder **Assets** het bestand `Dagverslag-windows-x64.zip`. Kies niet voor
**Source code (zip)** of **Source code (tar.gz)**; dat zijn alleen de bronbestanden en
bevatten de app niet. Pak de release-ZIP uit en dubbelklik op
`Dagverslag installeren.bat`. De installer zet de app in je lokale
`AppData\Local\Programs\Dagverslag`-map en plaatst een snelkoppeling met het appicoon
op je bureaublad. Daarna kun je Dagverslag gewoon vanaf je bureaublad openen. Je hoeft
geen .NET te installeren.

## Vanuit de broncode starten

Dit project bevat een Windows-desktopapp voor het invullen en opslaan van een dagverslag.

1. Installeer de .NET 8 SDK.
2. Open een terminal in de projectmap.
3. Start de app met:

   ```powershell
   dotnet run --project .\app\Werkdagboek.csproj
   ```

De app opent schermvullend met de naam **Lucas erf onderhoud**. Plaats het logo als
`logo.png` in `app\media`; het verschijnt dan rechtsboven. De app bevat geen fotovak.
Je kunt dag en datum invullen, uren bijhouden en noteren wat je hebt gedaan en
gebruikt en een naam toevoegen. Met **Dagverslag opslaan** bewaart de app het verslag
als `naam-dd-MM-jjjj.txt` in `Documenten\Lucas erf onderhoud\Dagverslagen`. Met
**Dagverslag openen** kun je eerder opgeslagen verslagen terughalen en bewerken.
Wijzigingen aan een geopend verslag worden onder de actuele naam en datum opgeslagen.
Bij een bestaande bestandsnaam vraagt de app eerst om bevestiging voordat deze wordt
overschreven.

## Een nieuwe GitHub-release maken

Push een versietag, bijvoorbeeld `v1.0.1`, of start de workflow **Windows-release**
handmatig via GitHub Actions en geef een versienummer op. GitHub Actions bouwt dan de
zelfstandige Windows-app en voegt `Dagverslag-windows-x64.zip` toe aan de GitHub-release.