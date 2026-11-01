namespace Rynzo.Client.Features.Crm.Models;

public sealed record CrmDeal(int Id, string Name, string Company, string Contact, string Role, string Stage, int Probability, decimal Value, string Owner, string Close, string CloseType, string Source, string Next, string Activity, string[] Tags, string[] Notes, string[] Tasks);

public static class CrmDealSource
{
    public static readonly string[] Owners = ["Sophia Carter", "Daniel Lee", "Olivia Smith", "Emma Wilson"];
    public static readonly string[] Stages = ["Qualification", "Discovery", "Proposal", "Negotiation", "Contract", "Closed Won", "Closed Lost", "Archived"];
    public static readonly CrmDeal[] Deals =
    [
        new(1048,"Enterprise Expansion","CloudAxis","Ava Thompson","VP Operations","Proposal",76,84500,"Sophia Carter","Sep 12","This Month","LinkedIn","Demo Follow-up","Proposal opened",["Enterprise","Expansion"],["Proposal is with the buying committee."],["Schedule pricing review"]),
        new(1049,"Automation Rollout","Vertex Labs","Liam Brooks","CTO","Negotiation",68,42000,"Daniel Lee","Sep 18","This Month","Referral","Security review","Pricing updated",["Strategic"],["Legal review begins this week."],["Send updated terms"]),
        new(1050,"Marketing Platform Upgrade","Rynzo Systems","Emma Davis","Head of Marketing","Contract",88,128000,"Olivia Smith","Sep 05","This Week","Event","Contract review","Contract uploaded",["Renewal","Upsell"],["Final approval expected soon."],["Confirm signature date"]),
        new(1051,"Support Renewal","BrightWorks","Noah Walker","Procurement Manager","Discovery",42,18000,"Emma Wilson","Oct 02","Next Month","Existing Customer","Needs discovery","Discovery call completed",["Renewal"],[],["Map requirements"])
    ];
}
