using UnityEngine;
using Assets.Enums;
using Assets.Classes;
using UnityEngine.UI;
using System.Collections;

public class VisualProcessor : MonoBehaviour {

    private static VisualProcessor _instance;
    private float _currTime;
    private bool _negative;
    private int _i;
    private VisualsDict _theme;
    public Texture blackImage;
    public static VisualProcessor Instance
    {
        get
        {
            return _instance;
        }
    }

    public RawImage ImageCanvas;

    [SerializeField]
    public VisualsDict[] Themes;

    void Update()
    {
        _currTime += Time.deltaTime;
        if (_theme != null && _theme.VisualTheme != Visuals_Enum.None)
        {
            loopDisplay(_theme);
        }
        
    }

    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Sets the current active theme
    /// </summary>
    /// <param name="visual"> the theme which is currently active</param>
    public void Display(Visuals_Enum visual)
    {
        
        foreach (var theme in Themes)
        {
            if (visual == theme.VisualTheme)
            {
                _negative = false;
                _i = 0;
                _theme = theme;
                break;
            } else
            {
                _theme = null;
                ImageCanvas.texture = blackImage;
            }
        }
    }

    /// <summary>
    /// showing the images in a loop to display an animation on the screen 
    /// </summary>
    /// <param name="theme"> the current theme </param>
    public void loopDisplay(VisualsDict theme)
    {
        if (_currTime >= 0.05)
        {
            if (_negative)
            {
                if (_i == 0)
                {
                    _negative = false;
                    return;
                }
                ImageCanvas.texture = theme.Textures[_i];
                _i--;
            }
            else
            {
                if (_i == theme.Textures.Length - 1)
                {
                    _negative = true;
                    return;
                }
                ImageCanvas.texture = theme.Textures[_i];
                _i++;
            }
            _currTime = 0;
        }
    }
}
