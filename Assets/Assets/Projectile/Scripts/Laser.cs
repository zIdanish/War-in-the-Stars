using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class Laser : MonoBehaviour
{
    public Shader Shader;
    public float Opacity;
    public float Speed;
    public Texture2D Texture;
    private Material temp;
    private SpriteRenderer sprite;
    private void Awake()
    {
        temp = new Material(Shader);

        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;
        sprite.material = temp;
    }
    private void OnDestroy()
    {
        Destroy(temp);
    }
    public void Begin(float Duration)
    {
        temp.SetFloat("_Size", transform.localScale.x);
        temp.SetFloat("_Elapsed", 0);
        temp.SetTexture("_Texture", Texture);
        temp.SetFloat("_Opacity", Opacity);
        temp.SetFloat("_Speed", Speed);
        sprite.enabled = true;

        StartCoroutine(Timeline(Duration));
    }
    private IEnumerator Timeline(float Duration)
    {
        float transition = Mathf.Min(0.5f, Duration/2);
        float wait = Duration - transition * 2;
        DOTween.To(
            () => 0.0f,
            x => temp.SetFloat("_Elapsed", x), 1.0f, transition
        ).SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(wait);

        DOTween.To(
            () => 1.0f,
            x => temp.SetFloat("_Elapsed", x), 0.0f, transition)
               .SetEase(Ease.InQuad);

        yield return new WaitForSeconds(transition);
        Destroy(this.gameObject);
    }
}
