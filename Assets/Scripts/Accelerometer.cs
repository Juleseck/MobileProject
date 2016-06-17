using Assets.Enums;
using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;

public class Accelerometer : MonoBehaviour
{
    
    public float TimeFrameData = 1;
    public float TimeFrameTrigger = 5;
    public float ZPositive = 0.2f;
    public float ZNegative = -1.0f;
    public int PeakCountMax = 6;
    public int PeakCountMin = 5;
    public GameObject cubeX, cubeY, cubeZ;
    public int MultiplicityTriggerValue = 3;

    private float _timePast;
    private List<AccelerationEvent> _data;
    private List<int> _positivePeaks;
    private List<int> _negativePeaks;
    private float[] _waveValuesNeeded;
    private float[] _jumpValuesNeeded;
    private List<float> _waveValues;
    private List<float> _jumpValues;
    private List<float> _checkedWaveValues;
    private List<float> _checkedJumpValues;
    private ClientScriptMin serverConnection;
    private float beginY;
    private List<Enum_Trigger> _actions;
    private Queue<KeyValuePair<double, Enum_Trigger>> triggersFiFo;

    void Start()
    {
        _actions = new List<Enum_Trigger>();
        _positivePeaks = new List<int>();
        _negativePeaks = new List<int>();
        _waveValues = new List<float>();
        _jumpValues = new List<float>();
        _waveValuesNeeded = new float[] { 1.0f, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f, 0.4f, 0.3f, 0.2f, 0.1f, 0.0f, -0.1f, -0.2f, -0.3f, -0.4f, -0.5f, -0.6f, -0.7f, -0.8f, -0.9f, -1.0f };
        _jumpValuesNeeded = new float[] { 1.4f, 1.3f, 1.2f, 1.1f, 1.0f, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f, 0.4f, 0.3f, 0.2f, 0.1f, 0.0f, -0.1f, -0.2f };
        foreach (float value in _waveValuesNeeded)
        {
            _waveValues.Add(value);
        }
        foreach (float value in _jumpValuesNeeded)
        {
            _jumpValues.Add(value);
        }
        _checkedWaveValues = new List<float>();
        _checkedJumpValues = new List<float>();
        _data = new List<AccelerationEvent>();
        serverConnection = GetComponent<ClientScriptMin>();
        triggersFiFo = new Queue<KeyValuePair<double, Enum_Trigger>>();
    }

    // Update is called once per frame
    void Update()
    {
        _timePast += Time.deltaTime;

        foreach (var acc in Input.accelerationEvents)
        {
            _data.Add(acc);
        }

        if (_timePast >= TimeFrameData)
        {
            
            beginY = Input.acceleration.y;
            var trigger = GestureDetection();

            if(trigger != Enum_Trigger.Null)
            {
                ProcessTrigger(trigger);
            }
            
            _checkedWaveValues.Clear();
            _checkedJumpValues.Clear();
            _data.Clear();
            _positivePeaks.Clear();
            _negativePeaks.Clear();
            _timePast = 0;
        }

        CleanQueue();
        //Debug.Log(Input.acceleration);
        //Debug.Log(Input.acceleration.magnitude);
    }

    public void Reset()
    {
        cubeX.transform.position = new Vector3(0,0,0);
        cubeY.transform.position = new Vector3(0, 0, 0);
        cubeZ.transform.position = new Vector3(0, 0, 0);
    }

    /**
     * 
     **/
    private Enum_Trigger GestureDetection()
    {
        bool klappen = true;
        bool jump = false;
        bool wave = false;
        foreach (AccelerationEvent acc in _data)
        {
            var zPosition = acc.acceleration.z;

            if (zPosition > ZPositive)
            {
                _positivePeaks.Add(1);
            }
            if (zPosition < ZNegative)
            {
                _negativePeaks.Add(1);
            }

            var y = (float)Math.Round(acc.acceleration.y, 1);

            if ((beginY + 2.0f) < y || (beginY - 2.0f) > y)
                klappen = false;

            if (_waveValues.Contains(y))
            {
                if (!_checkedWaveValues.Contains(y))
                {
                    _checkedWaveValues.Add(y);
                }
            }

            var z = (float)Math.Round(acc.acceleration.z, 1);
            if (z == -1.0f || z == -0.9f || z == -0.8f)
            {
                wave = true;
            }

            var x = (float)Math.Round(acc.acceleration.x, 1);

            if (_jumpValues.Contains(x))
            {
                if (!_checkedJumpValues.Contains(x))
                {
                    _checkedJumpValues.Add(x);
                }
                if (x == 1.0f)
                    jump = true;
            }
        }
        //Debug.Log("Waves: " + _checkedWaveValues.Count + " - jumps: " + _checkedJumpValues.Count);
        if (klappen && _positivePeaks.Count > PeakCountMax && _negativePeaks.Count > PeakCountMin)
        {
            return Enum_Trigger.Klappen;
        }
        if (wave && !klappen && _checkedWaveValues.Count > 12)
        {
            return Enum_Trigger.Wave;
        }

        if (!wave && jump && _checkedJumpValues.Count > 10)
        {
            return Enum_Trigger.Springen;
        }

        return Enum_Trigger.Null;
    }

    //clear triggers that have been in the list for over 1 second.
    public void CleanQueue()
    {
        //current time - 1 second buffer zone
        float currentTime = Time.time - 5;

        while (triggersFiFo.Count > 0)
        {
            var kvp = triggersFiFo.Peek();

            if (kvp.Key <= currentTime)
            {
                triggersFiFo.Dequeue();
                CountdownTrigger(kvp.Value);
            }
            else {
                return;
            }
        }
    }

    //when trigger is removed subtract 1 from counter.
    public void CountdownTrigger(Enum_Trigger trigger)
    {
        switch (trigger)
        {
            case Enum_Trigger.Klappen:
                TriggerCounterMobile.DecreaseKlappen();
                break;
            case Enum_Trigger.Springen:
                TriggerCounterMobile.DecreaseSpringen();
                break;
            case Enum_Trigger.Wave:
                TriggerCounterMobile.DecreaseWave();
                break;
        }
    }

    //add trigger to queue and add 1 to triggerCounter.
    public void ProcessTrigger(Enum_Trigger trigger)
    {
        var correctTrigger = checkKlappenWave(trigger);
        serverConnection.SendTrigger(correctTrigger);

        triggersFiFo.Enqueue(new KeyValuePair<double, Enum_Trigger>(Time.time, trigger));

        switch (trigger)
        {
            case Enum_Trigger.Klappen:
                TriggerCounterMobile.IncreaseKlappen();
                //Debug.Log("Klappen");
                break;
            case Enum_Trigger.Springen:
                TriggerCounterMobile.IncreaseSpringen();
                //Debug.Log("Springen");
                break;
            case Enum_Trigger.Wave:
                TriggerCounterMobile.IncreaseWave();       
                break;
        }
    }

    private Enum_Trigger checkKlappenWave(Enum_Trigger trigger)
    {
        var result = trigger;

        if (TriggerCounterMobile.Klappen > TriggerCounterMobile.Wave)
        {
            result = Enum_Trigger.Klappen;
        }
        else if (TriggerCounterMobile.Wave > TriggerCounterMobile.Klappen)
        {
            result = Enum_Trigger.Wave;
        }
        else if (result == Enum_Trigger.Springen)
        {
            result = Enum_Trigger.Springen;
        }
        Debug.Log(result.ToString());
        return result;
    }
}


