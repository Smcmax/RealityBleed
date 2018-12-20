using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class FeedbackEvent : UnityEvent<Transform, int, Color> { }