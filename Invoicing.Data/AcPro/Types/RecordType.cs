using System.ComponentModel;

namespace Invoicing.Data.AcPro.Types
{
    public enum RecordType
    {
        [Description("Unknown")]
        Unknown = 0,

        [Description("Requirement Analysis")]
        RequirementAnalysis = 1,

        [Description("Design")]
        Design = 2,

        [Description("Implementation")]
        Implementation = 3,

        [Description("Bug Fixing")]
        BugFixing = 4,

        [Description("Project Management")]
        ProjectManagement = 5,

        [Description("Testing")]
        Testing = 6,

        [Description("Other")]
        Other = 7,

        [Description("Meeting")]
        Meeting = 8,

        [Description("Travel")]
        Travel = 9,

        [Description("Documentation")]
        Documentation = 10,

        [Description("External Meeting")]
        ExternalMeeting = 11,

        [Description("Internal Meeting")]
        InternalMeeting = 12,

        [Description("Research")]
        Research = 13,

        [Description("Jour Fixe")]
        JourFixe = 14,

        [Description("Jour Fixe (Technical)")]
        JourFixeTechnical = 15,

        [Description("Not to be billed")]
        NotToBeBilled = 16,

        [Description("Technical maintenance")]
        TechnicalMaintenance = 17,

        [Description("Learning")]
        Learning = 18,

		[Description("Support")]
		Support = 19,
	}
}
