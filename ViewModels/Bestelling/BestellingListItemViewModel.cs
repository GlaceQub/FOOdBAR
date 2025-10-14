using System;

public class BestellingListItemViewModel
{
    public int Id { get; set; }
    public string ReservatieInfo { get; set; } = string.Empty;
    public string ProductNaam { get; set; } = string.Empty;
    public int Aantal { get; set; }
    public string? Opmerking { get; set; }
    public DateTime TijdstipBestelling { get; set; }
    public string StatusNaam { get; set; } = string.Empty;
}