using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellUI : MonoBehaviour
{
    public GameObject icon;
    public RectTransform cooldown;
    public TextMeshProUGUI manacost;
    public TextMeshProUGUI damage;
    public TextMeshProUGUI spellName; //i want an in-game name display i feel like it'll be nice
    public TextMeshProUGUI numberAssignment; //hovering number over spell display so player knows what # to hit to switch to that spell
    public int numberAss; //temp holder variable
    public GameObject highlight;
    public Spell spell; //array set up mightve been clunky and unnecessary, gonna just try to keep array stuffs to SpellUIContainer
    float last_text_update;
    const float UPDATE_DELAY = 1;
    public GameObject dropbutton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        last_text_update = 0;
    }

    public void SetSpell(Spell spell, int index)
    {
        this.spell = spell;
        numberAss = index + 1;
        if (this.spell != null)
        {
            GameManager.Instance.spellIconManager.PlaceSprite(this.spell.GetIcon(), icon.GetComponent<Image>());
            UpdateText();
        }
    }
    void UpdateText()
    {
        if (this.spell != null)
        {
            manacost.text = this.spell.GetManaCost().ToString();
            damage.text = this.spell.GetDamage().ToString();
            spellName.text = this.spell.GetName();
            numberAssignment.text = numberAss.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.spell == null) return;
        if (Time.time > last_text_update + UPDATE_DELAY)
        {
            UpdateText();
            last_text_update = Time.time;
        }

        float since_last = Time.time - this.spell.last_cast;
        float perc;
        if (since_last > this.spell.GetCooldown())
        {
            perc = 0;
        }
        else
        {
            perc = 1 - since_last / this.spell.GetCooldown();
        }
        cooldown.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 48 * perc);
    }
}
