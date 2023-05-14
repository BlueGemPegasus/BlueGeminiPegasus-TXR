using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    public bool triggerBossFight = false;

    public Transform cylinder;

    public MeshRenderer[] beamMR;

    private bool beamOn = false;
    private bool toggleOn = true;
    private float counter = 0.8f;

    [SerializeField] private float _timer;

    private void Start()
    {
        _timer = 0;
    }

    private void Update()
    {

        if (!triggerBossFight)
            return;

        RotateCylinder();
        StartBattle();
    }

    private void StartBattle()
    {
        // After Five Minute ToggleBeam Off
        // After One Minutes ToggleBeam On
        if (!beamOn)
        {
            StartCoroutine(ToggleBeam());
            beamOn = true;
        }
        else if (_timer >= 300f && toggleOn)
        {
            StartCoroutine(ToggleBeam());
            toggleOn = false;
        }
        else if (_timer >= 360f && !toggleOn)
        {
            StartCoroutine(ToggleBeam());
            toggleOn = true;
            _timer = 0f;
            beamOn = false;
        }
        _timer += Time.deltaTime;
    }

    private void RotateCylinder()
    {
        cylinder.Rotate(Vector3.up, 1f);
    }

    // This is to toggle the Laser on and Off.
    private IEnumerator ToggleBeam()
    {
        if (beamMR.Length > 0)
        {
            Material[] materialOfBeam;
            foreach (MeshRenderer beam in beamMR)
            {
                materialOfBeam = beam.materials;

                if (materialOfBeam.Length > 0)
                {
                    if (!toggleOn)
                    {
                        while (materialOfBeam[0].GetFloat("_Metallic") >= 0)
                        {
                            counter -= 0.0125f;
                            for (int i = 0; i < materialOfBeam.Length; i++)
                            {
                                materialOfBeam[0].SetFloat("_Metallic", counter);
                            }
                            yield return new WaitForSeconds(0.025f);
                        }
                    }
                    else
                    {
                        while (materialOfBeam[0].GetFloat("_Metallic") <= 0.8)
                        {
                            counter += 0.0125f;
                            for (int i = 0; i<materialOfBeam.Length; i++)
                            {
                                materialOfBeam[0].SetFloat("_Metallic", counter);
                            }
                            yield return new WaitForSeconds(0.025f);
                        }
                    }
                }
            }
        }
    }
}
