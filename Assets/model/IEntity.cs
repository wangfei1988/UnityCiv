using UnityEngine;
using System.Collections;

public abstract class IEntity : MonoBehaviour {

    public AudioSource audioSource { get; protected set; }

    protected virtual void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public abstract void Select();
}
