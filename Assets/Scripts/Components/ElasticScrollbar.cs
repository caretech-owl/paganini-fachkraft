using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ElasticScrollbar : MonoBehaviour
{
    public Scrollbar scrollbar;
    public int numSteps = 5; // Set the total number of steps.
    private float stepSize;
    private int currentStep = 0;
    private bool isSliding = false;
    private float targetValue;

    private void Start()
    {
        stepSize = 1.0f / (numSteps-1);
    }

    private void Update()
    {
        if (isSliding)
        {
            // Smoothly approach the target value.
            scrollbar.value = Mathf.Lerp(scrollbar.value, targetValue, Time.deltaTime * 10f);

            // Check if the scrollbar value is close to the target value.
            if (Mathf.Approximately(scrollbar.value, targetValue))
            {
                isSliding = false;
            }
        }
    }
    private float prevValue = 0;
    public void OnScrollbarValueChanged()
    {
        if (isSliding) return;

        // Calculate the current step based on the scrollbar value.
        int step = Mathf.RoundToInt(scrollbar.value * (numSteps - 1) );
        float deltaValue = Mathf.Abs(prevValue - scrollbar.value);

        Debug.Log($"step: {step} value: {scrollbar.value} diff: {deltaValue}");        

        if (currentStep != step)
        {
            currentStep = step;
            ApplyElasticEffect(step);
        } else if (deltaValue < (Math.Pow(10,-4))) {
            Debug.Log("stopped///////////////////////");
            ApplyElasticEffect(step);
        }

        prevValue = scrollbar.value;
    }

    private void ApplyElasticEffect(int step)
    {
        isSliding = true;
        targetValue = stepSize * (step); //Mathf.Clamp01(scrollbar.value); // Clamp the target value within [0, 1].

        Debug.Log($"stepSize: {stepSize} step+1: {step + 1} TargetValue: {targetValue}");

        // Optionally, you can add a delay before the elastic effect starts.
        // You can use StartCoroutine to achieve this.
        // StartCoroutine(DelayedElasticEffect(0.5f));
    }

    // Optional Coroutine for adding a delay to the elastic effect.
    private IEnumerator DelayedElasticEffect(int step, float delay)
    {
        yield return new WaitForSeconds(delay);
        ApplyElasticEffect(step);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // This method is called when the user stops dragging the scrollbar handle.
        // You can apply the elastic effect here, if needed.
        // For example, you can call ApplyElasticEffect();

        Debug.Log(eventData);
    }
}
