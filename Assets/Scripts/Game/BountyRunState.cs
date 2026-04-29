using System;

[Serializable]
public class BountyRunState
{
    public BountyData[] OfferedBounties { get; private set; }
    public BountyData SelectedBounty { get; private set; }
    public BountyData SelectedFinalBoss { get; private set; }

    public bool HasChosenOpeningBounty { get; private set; }
    public bool MinibossDefeated { get; private set; }
    public bool FinalBossUnlocked { get; private set; }

    public BountyRunState()
    {
        OfferedBounties = Array.Empty<BountyData>();
    }

    public void SetOfferedBounties(BountyData[] offeredBounties)
    {
        OfferedBounties = offeredBounties ?? Array.Empty<BountyData>();
    }

    public void SelectOpeningBounty(BountyData bounty)
    {
        SelectedBounty = bounty;
        HasChosenOpeningBounty = bounty != null;
    }

    public void MarkMinibossDefeated()
    {
        MinibossDefeated = true;
        FinalBossUnlocked = true;
    }

    public void SelectFinalBoss(BountyData bounty)
    {
        if (!FinalBossUnlocked)
        {
            return;
        }

        SelectedFinalBoss = bounty;
    }

    public void Reset()
    {
        OfferedBounties = Array.Empty<BountyData>();
        SelectedBounty = null;
        SelectedFinalBoss = null;
        HasChosenOpeningBounty = false;
        MinibossDefeated = false;
        FinalBossUnlocked = false;
    }
}