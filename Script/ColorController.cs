using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class ColorController : MonoBehaviour
{
    //Make sure your GameObject has a Renderer component in the Inspector window
    [Header("マテリアル割り当て")]
    [SerializeField]
    private Material InstanceMaterial;

    [Header("HSV value")]
    [SerializeField]
    private float m_Hue;
    [SerializeField]
    private float m_Saturation;
    [SerializeField]
    private float m_Value;

    [Header("Intensity")]
    [SerializeField]
    private float m_Intensity;

    [Header("Bloom")]
    [SerializeField]
    private float m_Bloom;

    [Header("Diffusion")]
    [SerializeField]
    private float m_Diffusion;

    //These are the Sliders that control the values. Remember to attach them in the Inspector window.
    [Header("HSVスライダー")]
    public Slider m_SliderHue;
    public Slider m_SliderSaturation;
    public Slider m_SliderValue;

    [Header("Intensityスライダー")]
    public Slider m_SliderIntensity;

    [Header("Bloomスライダー")]
    public Slider m_SliderBloom;

    [Header("Diffusionスライダー")]
    public Slider m_SliderDiffusion;

    [Header("ポストプロセッシング (PPS)")]
    [SerializeField]
    private PostProcessVolume PPvolume = null;

    [Header("テキスト")]
    public Text m_Huetext;
    public Text m_Saturationtext;
    public Text m_Valuetext;
    public Text m_Intensitytext;
    public Text p_Bloomtext;
    public Text p_Diffusiontext;

    void Start()
    {
        //Set the maximum and minimum values for the Sliders
        m_SliderHue.maxValue = 360;
        m_SliderSaturation.maxValue = 100;
        m_SliderValue.maxValue = 100;
        m_SliderIntensity.maxValue = 10;
        m_SliderBloom.maxValue = 20;
        m_SliderDiffusion.maxValue = 10;

        m_SliderHue.minValue = 0;
        m_SliderSaturation.minValue = 0;
        m_SliderValue.minValue = 0;
        m_SliderIntensity.minValue = 0;
        m_SliderBloom.minValue = 0;
        m_SliderDiffusion.minValue = 1;

        InstanceMaterial.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        //These are the Sliders that determine the amount of the hue, saturation and value in the Color
        m_Hue = m_SliderHue.value;
        m_Saturation = m_SliderSaturation.value;
        m_Value = m_SliderValue.value;
        m_Intensity = m_SliderIntensity.value;
        m_Bloom = m_SliderBloom.value;
        m_Diffusion = m_SliderDiffusion.value;

        float factor = Mathf.Pow(2, m_Intensity);

        float H = (m_Hue / m_SliderHue.maxValue);
        float S = (m_Saturation / m_SliderSaturation.maxValue);
        float V = (m_Value / m_SliderValue.maxValue);

        m_Huetext.text = m_Hue.ToString();
        m_Saturationtext.text = m_Saturation.ToString();
        m_Valuetext.text = m_Value.ToString();
        m_Intensitytext.text = m_Intensity.ToString("f2");
        p_Bloomtext.text = m_Bloom.ToString("f2");
        p_Diffusiontext.text = m_Diffusion.ToString("f2");

        //Create an RGB color from the HSV values from the Sliders
        //Change the Color of your GameObject to the new Color
        InstanceMaterial.SetColor("_EmissionColor", Color.HSVToRGB(H, S, V) * factor);

        Bloom bloom = PPvolume.profile.GetSetting<Bloom>();
        bloom.intensity.value = m_Bloom;
        bloom.diffusion.value = m_Diffusion;
    }
}
