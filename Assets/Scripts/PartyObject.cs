// PartyObject
// defines class for party object in party menu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyObject
{
    // creates variables to handle PartyObject information
    public Sprite portrait;

    // creates constructor for PartyObject based on required contents
    public PartyObject(Sprite newPortrait)
    {
        // constructs PartyObject based on passed contents
        portrait = newPortrait;
    }

    // creates constructor for PartyObject which copies another PartyObject
    public PartyObject(PartyObject partyObject)
    {
        // constructs PartyObject based on passed PartyObject
        portrait = partyObject.portrait;
    }
}
