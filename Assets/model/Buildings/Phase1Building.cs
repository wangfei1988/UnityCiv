﻿using UnityEngine;
using System.Collections;

public class Phase1Building : MonoBehaviour {

    public string Name;
    public Sprite Icon;
    public string Tooltip;
    public Tile Location;

    public AudioSource audioSource { get; protected set; }
    protected virtual void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.maxDistance = 20;
        audioSource.minDistance = 2;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }
}
