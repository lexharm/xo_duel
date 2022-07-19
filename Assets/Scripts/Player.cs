using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private string sign;

    public string GetSign()
    {
        return sign;
    }
}
