# LucasBullshit

## Dagverslag downloaden en starten

Open [Releases](https://github.com/ScorchingGhost/LucasBullshit/releases) en download
`Dagverslag-windows-x64.zip` van de nieuwste release. Pak het ZIP-bestand uit en start
`Dagverslag.exe`. De app en het logo moeten samen in de uitgepakte map blijven. Je hoeft
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

Push een versietag, bijvoorbeeld `v1.0.0`. GitHub Actions bouwt dan automatisch de
zelfstandige Windows-app en voegt `Dagverslag-windows-x64.zip` toe aan de GitHub-release.