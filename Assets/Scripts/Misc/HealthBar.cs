
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    public GameObject enemy;
    public float offset;
    public bool fade = false;
    public float fadeDur = 0.8f;
    public Vector3 rotationEuler;

    private SpriteRenderer[] renderers;
    public static HealthBar Create(Vector2 position, Vector2 size, float offset, GameObject enemy, Sprite whitePixelSprite)
    {
        GameObject healthBarGameObject = new GameObject("HealthBar");
        healthBarGameObject.transform.position = position;
        GameObject backgroundGameObject = new GameObject("background", typeof(SpriteRenderer));
        backgroundGameObject.transform.SetParent(healthBarGameObject.transform);
        backgroundGameObject.transform.localPosition = Vector3.zero;
        backgroundGameObject.transform.localScale = size;
        backgroundGameObject.GetComponent<SpriteRenderer>().color = Color.grey;
        backgroundGameObject.GetComponent<SpriteRenderer>().sprite = whitePixelSprite;
        backgroundGameObject.GetComponent<SpriteRenderer>().sortingOrder = 100;
        GameObject barGameObject = new GameObject("Bar");
        barGameObject.transform.SetParent(healthBarGameObject.transform);
        barGameObject.transform.localPosition = new Vector2(-40 / 2f, 0f);
        GameObject barSpriteGameObject = new GameObject("BarSprite", typeof(SpriteRenderer));
        barSpriteGameObject.transform.SetParent(barGameObject.transform);
        barSpriteGameObject.transform.localPosition = new Vector2(40 / 2f, 0f);
        barSpriteGameObject.transform.localScale = size;
        barSpriteGameObject.GetComponent<SpriteRenderer>().color = Color.red;
        barSpriteGameObject.GetComponent<SpriteRenderer>().sprite = whitePixelSprite;
        barSpriteGameObject.GetComponent<SpriteRenderer>().sortingOrder = 110;

        HealthBar healthBar = healthBarGameObject.AddComponent<HealthBar>();
        healthBar.enemy = enemy;
        healthBar.offset = offset;
        return healthBar;
    }
    private Transform bar;
    // Start is called before the first frame update
    void Awake()
    {
        bar = transform.Find("Bar");
    }

    // Update is called once per frame
    void Update()
    {
        resetPosition();
    }

    public void setSize(float sizeNormalized)
    {
        bar.localScale = new Vector3(sizeNormalized, 1f);
        if (fade)
        {
            if (renderers == null)
                renderers = GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer rend in renderers)
            {
                Color toChange = rend.color;
                Color newColor = new Color(toChange.r, toChange.g, toChange.b, 1);
                rend.color = newColor;
                DOTween.To(() => rend.color.a
                , x => rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, x)
                , 0, fadeDur);
            }


        }
    }

    private void Hide()
    {

        if (renderers == null)
            renderers = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer rend in renderers)
        {
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0);
        }

    }

    public void DoFade(float fadeDur)
    {
        if (fadeDur > 0f)
        {
            fade = true;
            this.fadeDur = fadeDur;
            Hide();

        }
        else { fade = false; }
    }

    public void resetPosition()
    {
        gameObject.transform.position = new Vector2(enemy.transform.position.x, enemy.transform.position.y + offset);
        transform.rotation = Quaternion.identity;
        //gameObject.transform.rotation = enemy.transform.rotation;
    }


    public void Die()
    {
        Object.Destroy(gameObject);
    }
}
