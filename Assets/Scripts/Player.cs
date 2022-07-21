using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Color color;
    [SerializeField] private string sign;

    public Color GetColor()
    {
        return color;
    }

    public string GetSign()
    {
        return sign;
    }
}
