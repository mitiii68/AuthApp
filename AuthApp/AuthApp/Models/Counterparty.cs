using AuthApp.Enums;

namespace AuthApp.Models;

public class Counterparty
{
    public int Id { get; set; }  

    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string? OtherPhone { get; set; }
    public string? MainPhone { get; set; }
    public string? Website { get; set; }
    public string? MainEmail { get; set; }
    public string? Fax { get; set; }
    public DateTime? CreatedAt { get; set; }

    public CounterpartyType? Type { get; set; }
    public Industry? Industry { get; set; }
    public OrganizationType? OrganizationType { get; set; }

    public string? IinBin { get; set; }
    public bool DoNotSendEmail { get; set; }

    public string? FacebookUrl { get; set; }
    public string? VkontakteUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public DateTime? LastSmsDate { get; set; }

    public string? LegalAddress { get; set; }
    public string? ActualAddress { get; set; }
    public string? Country { get; set; }
    public string? Region { get; set; }
    public string? City { get; set; }
    public string? ActualCountry { get; set; }
    public string? ActualRegion { get; set; }
    public string? ActualCity { get; set; }
}