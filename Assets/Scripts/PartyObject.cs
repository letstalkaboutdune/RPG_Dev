// PartyObject
// defines class for party object in party menu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyObject
{
    // creates variables to handle PartyObject information
    public CharStats stats;
    public string name;
    public int order;
    public Sprite portrait;

    // creates constructor for PartyObject based on required contents
    public PartyObject(CharStats stats, string name, int order)
    {
        // constructs PartyObject based on passed contents
        this.stats = stats;
        this.name = name;
        this.order = order;
        this.portrait = Resources.Load<Sprite>("Sprites/Portraits/" + name);
    }

    // creates constructor for PartyObject which copies another PartyObject
    public PartyObject(PartyObject partyObject)
    {
        // constructs PartyObject based on passed PartyObject
        this.stats = partyObject.stats;
        this.name = partyObject.name;
        this.order = partyObject.order;
        this.portrait = Resources.Load<Sprite>("Sprites/Portraits/" + name);
    }
}
