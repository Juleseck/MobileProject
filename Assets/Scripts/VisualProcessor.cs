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
            //Debug.Log(_i);
        }
    }

    IEnumerator wait(Texture t)
    {
        yield return new WaitForSeconds(1f);
        ImageCanvas.texture = t;
    }
}
