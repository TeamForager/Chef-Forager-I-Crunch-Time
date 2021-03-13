using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonPlayerHealth : MonoBehaviour
{

    [SerializeField] private float maxHealth;
    [SerializeField] private GameObject hpBarUI;
    [SerializeField] private float invincibilityDurationSeconds;
    [SerializeField] private GameObject model;
    private GameObject hpBar;
    private Image hpBarFull;
    private Text hpText;
    private bool isInvincible;
    private float invincibilityDeltaTime = 0.025f;

    private Renderer rend;
    private Color[] originalColors;
    private Color onDamageColor = Color.white;

    private float health;

    // Start is called before the first frame update
    void Start()
    {
        isInvincible = false;
        health = maxHealth;
        hpBar = hpBarUI;
        hpBarFull = hpBar.transform.Find("HpBar").gameObject.GetComponent<Image>();
        hpText = hpBar.transform.Find("HpText").gameObject.GetComponent<Text>();
        hpBarFull.fillAmount = health / maxHealth;
        hpText.text = health + "/" + maxHealth;
        SetupFlash();
    }

    private void SetupFlash()
    {
        rend = model.GetComponentInChildren<Renderer>();
        originalColors = new Color[rend.materials.Length];
        for (var i = 0; i < rend.materials.Length; i++)
        {
            originalColors[i] = rend.materials[i].color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void HandleHit(float damage)
    {
        if (isInvincible) return;

        this.health -= damage;
        hpBarFull.fillAmount = health / maxHealth;
        hpText.text = health + "/" + maxHealth;
        //StartCoroutine(FlashOnDamage());

        if (this.health <= 0)
        {
           //Die();
        }
        StartCoroutine(BecomeTemporarilyInvincible());
    }

    private IEnumerator BecomeTemporarilyInvincible()
    {
        Debug.Log("Player turned invincible!");
        isInvincible = true;
        bool isFlashing = false;
        for (float i = 0; i < invincibilityDurationSeconds; i += invincibilityDeltaTime)
        {
            // Alternate between 0 and 1 scale to simulate flashing
            if (isFlashing)
            {
                for (var k = 0; k < rend.materials.Length; k++)
                {
                    rend.materials[k].color = onDamageColor;
                }
            }
            else
            {
                for (var k = 0; k < rend.materials.Length; k++)
                {
                    rend.materials[k].color = originalColors[k];
                }
            }
            isFlashing = !isFlashing;
            yield return new WaitForSeconds(invincibilityDeltaTime);
        }
        Debug.Log("Player is no longer invincible!");
        isInvincible = false;
    }

    private void ScaleModelTo(Vector3 scale)
    {
        model.transform.localScale = scale;
    }
}