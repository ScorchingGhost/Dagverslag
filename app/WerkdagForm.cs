using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Werkdagboek;

internal sealed class WerkdagForm : Form
{
    private static readonly Color Ink = Color.FromArgb(34, 48, 56);
    private static readonly Color Muted = Color.FromArgb(111, 126, 130);
    private static readonly Color Teal = Color.FromArgb(28, 112, 103);
    private static readonly Color Paper = Color.FromArgb(246, 247, 243);
    private static readonly Color Line = Color.FromArgb(222, 229, 224);
    private readonly DateTimePicker datePicker = new();
    private readonly TextBox nameInput = new();
    private readonly NumericUpDown hoursInput = new();
    private readonly TextBox workInput = new();
    private readonly TextBox materialsInput = new();
    private readonly PictureBox logoPicture = new();
    private readonly Label statusLabel = new();
    private Icon? logoIcon;
    private readonly string logoPath = Path.Combine(AppContext.BaseDirectory, "media", "logo.png");
    private readonly string reportsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Lucas erf onderhoud",
        "Dagverslagen");
    private string? currentReportPath;
    private bool isDirty;

    public WerkdagForm()
    {
        Text = "Dagverslag 1.0";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(860, 760);
        WindowState = FormWindowState.Maximized;
        BackColor = Paper;
        Font = new Font("Segoe UI", 10);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(34, 24, 34, 20),
            BackColor = Paper
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 112));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        Controls.Add(root);

        root.Controls.Add(CreateHeader(), 0, 0);

        var scroll = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Paper,
            Padding = new Padding(0, 8, 8, 8)
        };
        root.Controls.Add(scroll, 0, 1);
        var formContent = CreateFormContent();
        scroll.Controls.Add(formContent);
        void ResizeFormContent()
        {
            formContent.Width = Math.Max(0, scroll.ClientSize.Width - scroll.Padding.Horizontal);
            formContent.Height = Math.Max(720, scroll.ClientSize.Height - scroll.Padding.Vertical);
        }
        scroll.Resize += (_, _) => ResizeFormContent();
        ResizeFormContent();

        root.Controls.Add(CreateFooter(), 0, 2);

        datePicker.Value = DateTime.Today;
        datePicker.ValueChanged += (_, _) => UpdateDateFormat();
        datePicker.ValueChanged += (_, _) => MarkDirty();
        nameInput.TextChanged += (_, _) => MarkDirty();
        hoursInput.ValueChanged += (_, _) => MarkDirty();
        workInput.TextChanged += (_, _) => MarkDirty();
        materialsInput.TextChanged += (_, _) => MarkDirty();
        UpdateDateFormat();
    }

    private Control CreateHeader()
    {
        var header = new Panel { Dock = DockStyle.Fill, BackColor = Paper };
        var titleArea = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Paper
        };
        var eyebrow = new Label
        {
            Text = "DAGVERSLAG",
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Teal,
            AutoSize = true,
            Location = new Point(0, 4)
        };
        var title = new Label
        {
            Text = "Lucas erf onderhoud",
            Font = new Font("Segoe UI Semibold", 30, FontStyle.Bold),
            ForeColor = Ink,
            AutoSize = true,
            Location = new Point(0, 30)
        };
        titleArea.Controls.AddRange([eyebrow, title]);

        logoPicture.Size = new Size(140, 88);
        logoPicture.SizeMode = PictureBoxSizeMode.Zoom;
        logoPicture.BackColor = Color.Transparent;
        logoPicture.BorderStyle = BorderStyle.None;
        logoPicture.Dock = DockStyle.Right;
        logoPicture.Margin = new Padding(12, 0, 0, 0);
        logoPicture.Paint += (_, e) =>
        {
            if (logoPicture.Image is null)
                TextRenderer.DrawText(e.Graphics, "media\\logo.png", new Font("Segoe UI", 9),
                    logoPicture.ClientRectangle, Muted,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        };
        LoadLogo();

        header.Controls.Add(titleArea);
        header.Controls.Add(logoPicture);
        return header;
    }

    private Control CreateFormContent()
    {
        var content = new TableLayoutPanel
        {
            Height = 720,
            ColumnCount = 2,
            RowCount = 2,
            BackColor = Paper,
            Dock = DockStyle.Top,
            Padding = new Padding(0, 0, 12, 0),
            Margin = Padding.Empty
        };
        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 116));
        content.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var detailsLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 2,
            Padding = new Padding(20, 12, 20, 12),
            Margin = new Padding(0, 0, 0, 16),
            BackColor = Color.White
        };
        detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
        detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
        detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.334F));
        detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
        detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        detailsLayout.Controls.Add(CreateFieldLabel("NAAM"), 0, 0);
        detailsLayout.Controls.Add(CreateFieldLabel("UREN GEWERKT"), 1, 0);
        detailsLayout.Controls.Add(CreateFieldLabel("DATUM"), 2, 0);
        nameInput.Font = new Font("Segoe UI", 12);
        nameInput.BorderStyle = BorderStyle.FixedSingle;
        nameInput.Dock = DockStyle.Fill;
        nameInput.Margin = new Padding(0, 6, 12, 0);
        nameInput.PlaceholderText = "Naam";
        detailsLayout.Controls.Add(nameInput, 0, 1);
        datePicker.Format = DateTimePickerFormat.Custom;
        datePicker.Font = new Font("Segoe UI", 12);
        datePicker.CalendarMonthBackground = Color.White;
        datePicker.Dock = DockStyle.Fill;
        datePicker.Margin = new Padding(12, 5, 0, 0);
        detailsLayout.Controls.Add(datePicker, 2, 1);
        hoursInput.DecimalPlaces = 2;
        hoursInput.Increment = 0.25M;
        hoursInput.Maximum = 24;
        hoursInput.Minimum = 0;
        hoursInput.Font = new Font("Segoe UI", 15);
        hoursInput.Dock = DockStyle.Fill;
        hoursInput.Width = 180;
        hoursInput.Margin = new Padding(12, 6, 12, 0);
        detailsLayout.Controls.Add(hoursInput, 1, 1);
        content.Controls.Add(detailsLayout, 0, 0);
        content.SetColumnSpan(detailsLayout, 2);

        content.Controls.Add(
            CreateTextCard("WERKZAAMHEDEN", "Beschrijf wat je vandaag hebt gedaan...", workInput),
            0, 1);
        content.Controls.Add(
            CreateTextCard("GEBRUIKTE MATERIALEN & MIDDELEN", "Schrijf op wat je hebt gebruikt...", materialsInput),
            1, 1);

        return content;
    }

    private void LoadLogo()
    {
        if (!File.Exists(logoPath))
            return;

        try
        {
            using var stream = File.OpenRead(logoPath);
            using var source = Image.FromStream(stream);
            using var logo = new Bitmap(source);
            logoPicture.Image = new Bitmap(logo);

            var iconHandle = logo.GetHicon();
            try
            {
                using var temporaryIcon = Icon.FromHandle(iconHandle);
                logoIcon = (Icon)temporaryIcon.Clone();
                Icon = logoIcon;
            }
            finally
            {
                DestroyIcon(iconHandle);
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException or OutOfMemoryException)
        {
            MessageBox.Show(this, $"Het logo kon niet worden geladen.\n{logoPath}\n{exception.Message}",
                "Logo laden", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyIcon(IntPtr handle);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            logoPicture.Image?.Dispose();
            logoIcon?.Dispose();
        }

        base.Dispose(disposing);
    }

    private Control CreateTextCard(string title, string placeholder, TextBox input)
    {
        var card = CreateCard();
        card.Margin = new Padding(0, 0, 8, 0);
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(20, 14, 20, 14),
            BackColor = Color.White
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.Controls.Add(CreateFieldLabel(title), 0, 0);
        input.Multiline = true;
        input.AcceptsReturn = true;
        input.AcceptsTab = true;
        input.ScrollBars = ScrollBars.Vertical;
        input.BorderStyle = BorderStyle.FixedSingle;
        input.Font = new Font("Segoe UI", 11);
        input.ForeColor = Ink;
        input.BackColor = Color.White;
        input.Dock = DockStyle.Fill;
        input.Margin = new Padding(0, 3, 0, 0);
        input.PlaceholderText = placeholder;
        input.TextChanged += (_, _) => MarkDirty();
        layout.Controls.Add(input, 0, 1);
        card.Controls.Add(layout);
        return card;
    }

    private Control CreateFooter()
    {
        var footer = new Panel { Dock = DockStyle.Fill, BackColor = Paper };
        var saveButton = new Button
        {
            Text = "Dagverslag opslaan",
            Width = 190,
            Height = 44,
            Anchor = AnchorStyles.Right | AnchorStyles.Top,
            FlatStyle = FlatStyle.Flat,
            BackColor = Teal,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Location = new Point(Width - 300, 10)
        };
        saveButton.FlatAppearance.BorderSize = 0;
        saveButton.Click += (_, _) => SaveReport();
        footer.Resize += (_, _) => saveButton.Left = footer.ClientSize.Width - saveButton.Width;

        var openButton = new Button
        {
            Text = "Dagverslag openen",
            Width = 165,
            Height = 44,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Ink,
            Font = new Font("Segoe UI", 9),
            Cursor = Cursors.Hand,
            Location = new Point(140, 10)
        };
        openButton.FlatAppearance.BorderColor = Line;
        openButton.Click += (_, _) => OpenReport();

        var clearButton = new Button
        {
            Text = "Leeg formulier",
            Width = 130,
            Height = 44,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Ink,
            Font = new Font("Segoe UI", 9),
            Cursor = Cursors.Hand,
            Location = new Point(0, 10)
        };
        clearButton.FlatAppearance.BorderColor = Line;
        clearButton.Click += (_, _) => ClearForm();
        statusLabel.AutoSize = true;
        statusLabel.ForeColor = Teal;
        statusLabel.Font = new Font("Segoe UI", 9);
        statusLabel.Location = new Point(315, 23);
        footer.Controls.AddRange([clearButton, openButton, statusLabel, saveButton]);
        return footer;
    }

    private static Panel CreateCard() => new()
    {
        Dock = DockStyle.Fill,
        BackColor = Color.White,
        BorderStyle = BorderStyle.FixedSingle
    };

    private static Label CreateFieldLabel(string text) => new()
    {
        Text = text,
        Font = new Font("Segoe UI", 8, FontStyle.Bold),
        ForeColor = Muted,
        AutoSize = true,
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleLeft
    };

    private void UpdateDateFormat()
    {
        var day = datePicker.Value.DayOfWeek switch
        {
            DayOfWeek.Monday => "Maandag",
            DayOfWeek.Tuesday => "Dinsdag",
            DayOfWeek.Wednesday => "Woensdag",
            DayOfWeek.Thursday => "Donderdag",
            DayOfWeek.Friday => "Vrijdag",
            DayOfWeek.Saturday => "Zaterdag",
            DayOfWeek.Sunday => "Zondag",
            _ => throw new ArgumentOutOfRangeException(nameof(datePicker.Value))
        };
        var month = datePicker.Value.Month switch
        {
            1 => "januari",
            2 => "februari",
            3 => "maart",
            4 => "april",
            5 => "mei",
            6 => "juni",
            7 => "juli",
            8 => "augustus",
            9 => "september",
            10 => "oktober",
            11 => "november",
            12 => "december",
            _ => throw new ArgumentOutOfRangeException(nameof(datePicker.Value))
        };
        datePicker.CustomFormat = $"'{day}' d '{month}' yyyy";
    }

    private void SaveReport()
    {
        var missingField = string.IsNullOrWhiteSpace(nameInput.Text)
            ? "Naam"
            : hoursInput.Value <= 0
                ? "Uren gewerkt"
                : string.IsNullOrWhiteSpace(workInput.Text)
                    ? "Werkzaamheden"
                    : null;

        if (missingField is not null)
        {
            MessageBox.Show(this, $"Je bent het veld '{missingField}' vergeten in te vullen.",
                "Veld ontbreekt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!ConfirmSave())
            return;

        try
        {
            Directory.CreateDirectory(reportsDirectory);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show(this, $"De map voor dagverslagen kon niet worden aangemaakt.\n{reportsDirectory}\n{exception.Message}",
                "Map aanmaken mislukt", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var savePath = Path.Combine(reportsDirectory, $"{CreateReportFileName()}.txt");
        if (File.Exists(savePath) &&
            !string.Equals(Path.GetFullPath(currentReportPath ?? string.Empty), Path.GetFullPath(savePath), StringComparison.OrdinalIgnoreCase) &&
            MessageBox.Show(this, "Er bestaat al een dagverslag met deze naam en datum. Wil je het overschrijven?",
                "Dagverslag bestaat al", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            return;

        var culture = CultureInfo.GetCultureInfo("nl-NL");
        var report = new StringBuilder()
            .AppendLine("DAGVERSLAG")
            .AppendLine(new string('=', 40))
            .AppendLine($"Dag: {GetDutchWeekday(datePicker.Value.DayOfWeek)}")
            .AppendLine($"Datum: {datePicker.Value.ToString("d MMMM yyyy", culture)}")
            .AppendLine($"Naam: {nameInput.Text.Trim()}")
            .AppendLine($"Uren gewerkt: {hoursInput.Value.ToString("0.##", culture)}")
            .AppendLine()
            .AppendLine("WERKZAAMHEDEN")
            .AppendLine(workInput.Text.TrimEnd())
            .AppendLine()
            .AppendLine("GEBRUIKTE MATERIALEN & MIDDELEN")
            .AppendLine(materialsInput.Text.TrimEnd())
            .AppendLine()
            .AppendLine($"Logo: {(File.Exists(logoPath) ? logoPath : "niet toegevoegd")}")
            .ToString();

        try
        {
            File.WriteAllText(savePath, report, Encoding.UTF8);
            if (currentReportPath is not null &&
                !string.Equals(Path.GetFullPath(currentReportPath), Path.GetFullPath(savePath), StringComparison.OrdinalIgnoreCase) &&
                File.Exists(currentReportPath))
            {
                File.Delete(currentReportPath);
            }
            currentReportPath = savePath;
            isDirty = false;
            statusLabel.Text = "Dagverslag opgeslagen.";
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show(this, $"Het dagverslag kon niet worden opgeslagen.\n{exception.Message}",
                "Opslaan mislukt", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private bool ConfirmSave()
    {
        using var confirmation = new Form
        {
            Text = "Dagverslag opslaan",
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MinimizeBox = false,
            MaximizeBox = false,
            ShowInTaskbar = false,
            ClientSize = new Size(390, 145),
            BackColor = Paper,
            Font = new Font("Segoe UI", 10)
        };

        var message = new Label
        {
            Text = "Weet je zeker dat je dit dagverslag wilt opslaan?",
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleLeft,
            Location = new Point(22, 18),
            Size = new Size(346, 42),
            ForeColor = Ink
        };
        var saveButton = new Button
        {
            Text = "Opslaan",
            DialogResult = DialogResult.OK,
            Location = new Point(166, 83),
            Size = new Size(100, 38),
            FlatStyle = FlatStyle.Flat,
            BackColor = Teal,
            ForeColor = Color.White
        };
        saveButton.FlatAppearance.BorderSize = 0;
        var cancelButton = new Button
        {
            Text = "Annuleren",
            DialogResult = DialogResult.Cancel,
            Location = new Point(276, 83),
            Size = new Size(92, 38),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Ink
        };
        cancelButton.FlatAppearance.BorderColor = Line;
        confirmation.Controls.AddRange([message, saveButton, cancelButton]);
        confirmation.AcceptButton = saveButton;
        confirmation.CancelButton = cancelButton;
        return confirmation.ShowDialog(this) == DialogResult.OK;
    }

    private void OpenReport()
    {
        try
        {
            Directory.CreateDirectory(reportsDirectory);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show(this, $"De map met dagverslagen kon niet worden geopend.\n{reportsDirectory}\n{exception.Message}",
                "Map openen mislukt", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        using var dialog = new OpenFileDialog
        {
            Title = "Dagverslag openen",
            Filter = "Tekstbestanden|*.txt",
            InitialDirectory = reportsDirectory,
            CheckFileExists = true,
            Multiselect = false
        };
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;
        if (isDirty && MessageBox.Show(this, "Je hebt niet-opgeslagen wijzigingen. Wil je die weggooien en een ander dagverslag openen?",
                "Wijzigingen weggooien?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            return;

        try
        {
            LoadReport(File.ReadAllText(dialog.FileName, Encoding.UTF8));
            var selectedPath = Path.GetFullPath(dialog.FileName);
            var reportsRoot = Path.GetFullPath(reportsDirectory) + Path.DirectorySeparatorChar;
            currentReportPath = selectedPath.StartsWith(reportsRoot, StringComparison.OrdinalIgnoreCase)
                ? selectedPath
                : null;
            isDirty = false;
            statusLabel.Text = $"Geopend: {Path.GetFileName(dialog.FileName)}";
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or FormatException or ArgumentException)
        {
            MessageBox.Show(this, $"Het dagverslag kon niet worden geopend.\n{exception.Message}",
                "Openen mislukt", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadReport(string report)
    {
        var dateText = ReadHeaderValue(report, "Datum:");
        var culture = CultureInfo.GetCultureInfo("nl-NL");
        if (!DateTime.TryParse(dateText, culture, DateTimeStyles.None, out var date))
            throw new FormatException("De datum in dit dagverslag heeft een onbekend formaat.");

        var hoursText = ReadHeaderValue(report, "Uren gewerkt:");
        if (!decimal.TryParse(hoursText, NumberStyles.Number, culture, out var hours) ||
            hours < hoursInput.Minimum || hours > hoursInput.Maximum)
            throw new FormatException("Het aantal gewerkte uren in dit dagverslag is ongeldig.");

        var work = ReadSection(report, "WERKZAAMHEDEN", "GEBRUIKTE MATERIALEN & MIDDELEN");
        var materials = ReadSection(report, "GEBRUIKTE MATERIALEN & MIDDELEN", "Logo:");
        datePicker.Value = date;
        nameInput.Text = ReadOptionalHeaderValue(report, "Naam:");
        hoursInput.Value = hours;
        workInput.Text = work;
        materialsInput.Text = materials;
    }

    private static string ReadHeaderValue(string report, string header)
    {
        var line = report.Split(["\r\n", "\n"], StringSplitOptions.None)
            .FirstOrDefault(candidate => candidate.StartsWith(header, StringComparison.OrdinalIgnoreCase));
        if (line is null)
            throw new FormatException($"Het veld '{header.TrimEnd(':')}' ontbreekt in het dagverslag.");
        return line[header.Length..].Trim();
    }

    private static string ReadOptionalHeaderValue(string report, string header)
    {
        var line = report.Split(["\r\n", "\n"], StringSplitOptions.None)
            .FirstOrDefault(candidate => candidate.StartsWith(header, StringComparison.OrdinalIgnoreCase));
        return line is null ? string.Empty : line[header.Length..].Trim();
    }

    private string CreateReportFileName()
    {
        var name = string.Concat(nameInput.Text.Trim().Select(character =>
            Path.GetInvalidFileNameChars().Contains(character) || character < 32 ? '_' : character))
            .Trim(' ', '.');
        if (string.IsNullOrWhiteSpace(name))
            name = "Dagverslag";

        return $"{name}-{datePicker.Value:dd-MM-yyyy}";
    }

    private static string ReadSection(string report, string heading, string? nextHeading)
    {
        var headingIndex = report.IndexOf(heading, StringComparison.OrdinalIgnoreCase);
        if (headingIndex < 0)
            throw new FormatException($"Het onderdeel '{heading}' ontbreekt in het dagverslag.");

        var contentStart = headingIndex + heading.Length;
        while (contentStart < report.Length && (report[contentStart] == '\r' || report[contentStart] == '\n'))
            contentStart++;

        var contentEnd = nextHeading is null
            ? report.Length
            : report.IndexOf(nextHeading, contentStart, StringComparison.OrdinalIgnoreCase);
        if (contentEnd < 0)
            throw new FormatException($"Het onderdeel '{nextHeading}' ontbreekt in het dagverslag.");
        return report[contentStart..contentEnd].TrimEnd('\r', '\n');
    }

    private void MarkDirty()
    {
        isDirty = true;
        if (statusLabel.IsHandleCreated)
            statusLabel.Text = "Niet-opgeslagen wijzigingen.";
    }

    private static string GetDutchWeekday(DayOfWeek day) => day switch
    {
        DayOfWeek.Monday => "Maandag",
        DayOfWeek.Tuesday => "Dinsdag",
        DayOfWeek.Wednesday => "Woensdag",
        DayOfWeek.Thursday => "Donderdag",
        DayOfWeek.Friday => "Vrijdag",
        DayOfWeek.Saturday => "Zaterdag",
        DayOfWeek.Sunday => "Zondag",
        _ => throw new ArgumentOutOfRangeException(nameof(day))
    };

    private void ClearForm()
    {
        if (MessageBox.Show(this, "Weet je zeker dat je alle ingevulde gegevens wilt wissen?",
                "Formulier leegmaken", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        datePicker.Value = DateTime.Today;
        nameInput.Clear();
        hoursInput.Value = 0;
        workInput.Clear();
        materialsInput.Clear();
        currentReportPath = null;
        isDirty = false;
        statusLabel.Text = "Formulier is leeggemaakt.";
    }
}
