using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.MicrowaveReceiverController
{
    public class PeakScan
    {
        public event EventHandler<ScanCompleteEvent> ScanCompleted;

        private Dictionary<MicrowaveTuning, MicrowaveLinkQuality> _peakDict;
        /// <summary>
        /// Dictionary of Peak frequency to its signal strength
        /// </summary>
        public Dictionary<MicrowaveTuning, MicrowaveLinkQuality> PeakDictionary
        {
            get
            {
                return _peakDict;
            }
            set
            {
                _peakDict = value;
            }
        }

        private UInt64 _roughInterval = 15000000;
        /// <summary>
        /// Rough Interval, in Hz
        /// Interval of frequencies to check when doing a *rough* scan. 
        /// For example, with an interval of 15000000, scan would hit 1700MHz, 
        /// 1715MHz, 1730MHz when looking for a peak.
        /// </summary>
        public UInt64 RoughInterval
        {
            get
            {
                return _roughInterval;
            }
            set
            {
                _roughInterval = value;
            }
        }

        private UInt64 _fineInterval = 5000000;
        /// <summary>
        /// Fine Interval, in Hz
        /// Interval of frequencies to check when doing a *fine* scan. 
        /// For example, with an interval of 15000000, scan would hit 1700MHz, 
        /// 1715MHz, 1730MHz when looking for a peak.
        /// </summary>
        public UInt64 FineInterval
        {
            get
            {
                return _fineInterval;
            }
            set
            {
                _fineInterval = value;
            }
        }

        private MicrowaveReceiver rx;
        private bool inSearch = false;
        BackgroundWorker bgw = new BackgroundWorker();

        private MicrowaveTuning _prototype;
        private UInt64 _startFreq = 0;
        private UInt64 _endFreq = 0;
        private int _minStrength = 20;
        private int _passes = 3;
        /// <summary>
        /// Value 1-3 indicates whether to do a rough, fine, final scan.
        /// 3  = rough -> fine -> final. 
        /// Doing only a final will be more thorough, but will take longer. 
        /// Doing a rough or rough and fine first reduces the time, but could
        /// miss small peaks.
        /// </summary>
        public int Passes
        {
            get
            {
                return _passes;
            }
            set
            {
                _passes = value;
            }
        }

        /// <summary>
        /// Initializes PeakScan object using receiver to check signal strengths for
        /// different frequencies.
        /// </summary>
        /// <param name="receiver">Receiver to use when checking frequencies.</param>
        public PeakScan(MicrowaveReceiver receiver)
        {
            rx = receiver;
            bgw.WorkerSupportsCancellation = true;
            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
        }

        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ScanCompleteEvent args = new ScanCompleteEvent();
            args.EndFrequency = _endFreq;
            args.StartFrequency = _startFreq;
            args.MinSignalStrength = _minStrength;
            args.PeakDictionary = PeakDictionary;
            args.passes = Passes;
            if (e.Result != null && e.Result is Exception)
            {
                args.Error = e.Result as Exception;
                args.EndFrequency = 0;
                Debug.WriteLine("Scan interrupted by exception. Firing Event.");
                Debug.WriteLine("Exception details: " + args.Error.Message + Environment.NewLine +
                    args.Error.StackTrace);
            }
            else
            {
                Debug.WriteLine("Scan complete. Firing Event.");                
            }

            StringBuilder sb = new StringBuilder();
#if DEBUG
            foreach (KeyValuePair<MicrowaveTuning, MicrowaveLinkQuality> fr in PeakDictionary)
            {
                sb.AppendLine("Freq: " + fr.Key.FrequencyMHz + "   Strength: " + fr.Value.ReceivedCarrierLevel);
            }
            Debug.WriteLine(sb.ToString());
#endif

            if (this.ScanCompleted != null)
                ScanCompleted.Invoke(this, args);
            inSearch = false;
        }

        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                switch (_passes)
                {
                    case 1:
                        FinalScan();
                        break;
                    case 2:
                        List<UInt64> fineResults = FineScan();
                        FinalScan(fineResults);
                        break;
                    case 3:
                    default:
                        List<UInt64> roughResults = RoughScan();
                        fineResults = FineScan(roughResults);
                        FinalScan(fineResults);
                        break;
                }
            }
            catch (Exception exc)//Catch any exceptions that may occur from setting freqs
            {
                e.Result = exc;
            }
        }

        /// <summary>
        /// Cancels any pending scan. ScanComplete event may still fire
        /// with any results it has collected so far.
        /// </summary>
        public void CancelScan()
        {
            bgw.CancelAsync();
        }

        /// <summary>
        /// Does a scan asynchronously from start to end frequency. Number of passes
        /// for scan depends on <see cref="Passes"/>. If a frequency has a signal
        /// strength above <paramref name="MinStrength"/>, the highest frequency in the
        /// contiguous set of frequencies above MinStrength will be added to the dictionary.
        /// </summary>
        /// <param name="prototype">a prototype tuning that should be used when sweeping frequency</param>
        /// <param name="StartFreq">Frequency to start scanning at, inclusive, in Hz.</param>
        /// <param name="EndFreq">Frequency to stop scanning at, inclusive, in Hz.</param>
        /// <param name="MinStrength">Minimum signal strength required to register a peak.</param>
        public void PeakScanAsync(MicrowaveTuning prototype, UInt64 StartFreq, UInt64 EndFreq, int MinStrength)
        {
            if (bgw.IsBusy || inSearch)
            {
                bgw.CancelAsync();
            }
            //wait for cancellation
            while (bgw.IsBusy) 
                Thread.Sleep(100);

            inSearch = true;
            _prototype = prototype;
            _startFreq = StartFreq;
            _endFreq = EndFreq;
            _minStrength = MinStrength;
            _peakDict = new Dictionary<MicrowaveTuning, MicrowaveLinkQuality>();
            bgw.RunWorkerAsync();
        }

        /// <summary>
        /// Scans from last start to last end using <see cref="RoughInterval"/>, 
        /// putting rough results into PeakDictionary.
        /// </summary>
        /// <returns>List of start,end intervals of possible peaks.</returns>
        public List<UInt64> RoughScan()
        {
            List<UInt64> results = new List<UInt64>();
            PeakDictionary = new Dictionary<MicrowaveTuning, MicrowaveLinkQuality>();
            if (bgw.CancellationPending)
            {
                return results;
            }

            _prototype.Frequency = _startFreq;
            rx.SetTuning(_prototype);

            UInt64 prePeakStart = _startFreq;//Set on strength below
            UInt64 currentPeak = 0;
            MicrowaveLinkQuality currentPeakStrength = new MicrowaveLinkQuality(0);

            for (UInt64 currentFreq = _startFreq;
                (currentFreq < _endFreq + RoughInterval) && !bgw.CancellationPending;
                currentFreq += RoughInterval)
            {
                if (currentFreq > _endFreq)
                {
                    currentFreq = _endFreq;
                }
                _prototype.Frequency = currentFreq;
                try
                {
                    rx.SetTuning(_prototype);
                }
                catch
                {
                    continue;
                }

                MicrowaveLinkQuality strength = rx.GetLinkQuality();
                Debug.WriteLine("Freq Changed: " + currentFreq + "\r\nStrength: " + strength);

                if (strength.ReceivedCarrierLevel > _minStrength)
                {
                    if (strength.ReceivedCarrierLevel > currentPeakStrength.ReceivedCarrierLevel)//Save as highest peak so far
                    {
                        if (currentPeak > prePeakStart)//update to more accurate start of peak
                            prePeakStart = currentPeak;

                        currentPeak = currentFreq;
                        currentPeakStrength = strength;
                    }
                    else if (strength.ReceivedCarrierLevel == currentPeakStrength.ReceivedCarrierLevel)
                    {
                        //Let the interval roll on
                    }
                    else//Above minimum, but lower than last peak //strength < currentPeakStrength
                    {
                        PeakDictionary.Add(new MicrowaveTuning(_prototype), strength);
                        results.Add(prePeakStart);
                        results.Add(currentFreq);
                        Debug.WriteLine("Interval Added: " + prePeakStart + " to: " + currentFreq);

                        //Reset prePeak to catch higher peaks within same set
                        currentPeakStrength = strength;
                        prePeakStart = currentFreq;
                    }
                }
                else//Don't track peaks here, but make sure to start or end them if they are here
                {
                    //If tracking peak, save it
                    if (currentPeak > prePeakStart)
                    {
                        if (currentPeakStrength.ReceivedCarrierLevel > 0)
                        {
                            PeakDictionary.Add(new MicrowaveTuning(_prototype), new MicrowaveLinkQuality(currentPeakStrength));
                        }
                        results.Add(prePeakStart);
                        results.Add(currentFreq);
                        Debug.WriteLine("Interval Added: " + prePeakStart + " to: " + currentFreq);
                    }
                    //Reset prePeak to catch any peaks
                    currentPeakStrength.ReceivedCarrierLevel = 0;
                    prePeakStart = currentFreq;
                }
            }
            //Complete interval if still tracking peak
            if (currentPeak > prePeakStart)
            {
                if (currentPeakStrength.ReceivedCarrierLevel > 0)
                {
                    PeakDictionary.Add(new MicrowaveTuning(_prototype), new MicrowaveLinkQuality(currentPeakStrength)); 
                }
                results.Add(prePeakStart);
                results.Add(_endFreq);
                Debug.WriteLine("Interval Added: " + prePeakStart + " to: " + _endFreq);
            }

            return results;
        }

        /// <summary>
        /// Puts intermediate results into PeakDictionary
        /// </summary>
        /// <param name="roughIntervals"></param>
        /// <returns></returns>
        internal List<UInt64> FineScan(List<UInt64> roughIntervals)
        {
            List<UInt64> results = new List<UInt64>();
            if (bgw.CancellationPending) return results;
            Dictionary<MicrowaveTuning, MicrowaveLinkQuality> tempPeakDictionary = new Dictionary<MicrowaveTuning, MicrowaveLinkQuality>();
            //For each range in the roughIntervals
            for (int i = 0; i < roughIntervals.Count - 1 && !bgw.CancellationPending; i += 2)
            {
                UInt64 intStart = roughIntervals[i];
                UInt64 intEnd = roughIntervals[i + 1];
                UInt64 currentPeak = 0;
                MicrowaveLinkQuality currentPeakStrength = new MicrowaveLinkQuality(0);
                UInt64 prePeakFreq = intStart;

                Debug.WriteLine("Starting Interval: " + intStart + " to: " + intEnd);
                //For each frequency in the interval
                for (UInt64 CurrFreq = intStart;
                    CurrFreq < intEnd && !bgw.CancellationPending; CurrFreq += FineInterval)
                {

                    _prototype.Frequency = CurrFreq;
                    try
                    {
                        rx.SetTuning(_prototype);
                    }
                    catch
                    {
                        continue;
                    }
                    MicrowaveLinkQuality strength = rx.GetLinkQuality();
                    Debug.WriteLine("Freq Changed: " + CurrFreq + "\r\nStrength: " + strength.ReceivedCarrierLevel);

                    if (strength.ReceivedCarrierLevel > _minStrength)
                    {
                        if (strength.ReceivedCarrierLevel > currentPeakStrength.ReceivedCarrierLevel)//Save as highest peak so far
                        {
                            if (currentPeak > prePeakFreq)
                            {//If tracking a peak alrady, update start position
                                prePeakFreq = currentPeak;
                            }
                            //Save as highest peak
                            currentPeak = CurrFreq;
                            currentPeakStrength = strength;
                        }
                        else//above minimum, but lower than current peak
                        {
                            //Save peak and interval if we were tracking a peak
                            if (currentPeak > prePeakFreq)
                            {
                                tempPeakDictionary.Add(new MicrowaveTuning(_prototype), strength);
                                Debug.WriteLine("Temporary Peak Added: " + currentPeak);
                                results.Add(prePeakFreq);
                                results.Add(CurrFreq);
                                Debug.WriteLine("Interval Added: " + prePeakFreq + " to: " + CurrFreq);
                            }

                            //Reset prePeak to catch higher peaks within same set
                            currentPeakStrength = strength;
                            prePeakFreq = CurrFreq;
                        }
                    }
                    else//too low. Finish interval if one exists, but don't start any new ones.
                    {
                        //If tracking peak, end and save it
                        if (currentPeak > prePeakFreq)
                        {
                            if (currentPeakStrength.ReceivedCarrierLevel > 0)
                            {
                                tempPeakDictionary.Add(new MicrowaveTuning(_prototype), strength);
                            }
                            results.Add(prePeakFreq);
                            results.Add(CurrFreq);
                            Debug.WriteLine("Interval Added: " + prePeakFreq + " to: " + CurrFreq);
                        }
                        currentPeakStrength.ReceivedCarrierLevel = 0;
                        prePeakFreq = CurrFreq;
                    }
                }//End Frequencies within Interval

                if (currentPeak > prePeakFreq)
                {
                    if (currentPeakStrength.ReceivedCarrierLevel > 0)
                    {
                        //HACK this is inelegent
                        tempPeakDictionary.Add(new MicrowaveTuning(_prototype), new MicrowaveLinkQuality(currentPeakStrength));
                    }
                    results.Add(prePeakFreq);
                    results.Add(intEnd);
                    Debug.WriteLine("Interval Added: " + prePeakFreq + " to: " + intEnd);
                }
            }//End Intervals
            //Don't update the PeakDictionary if canellation is pending because we could have completed no
            //intervals and the old dictionary may still be better
            if (!bgw.CancellationPending)
                PeakDictionary = tempPeakDictionary;
            return results;
        }//End Fine Scan

        /// <summary>
        /// Scans from last start to last end using <see cref="FineInterval"/>, 
        /// putting fine results into PeakDictionary.
        /// </summary>
        /// <returns>List of start,end intervals of possible peaks.</returns>
        public List<UInt64> FineScan()
        {
            List<UInt64> interval = new List<UInt64>();
            interval.Add(_startFreq);
            interval.Add(_endFreq);
            return FineScan(interval);
        }

        /// <summary>
        /// Puts results into PeakDictionary for class
        /// Scans by 1s, e.g. 1700, 1701, etc
        /// </summary>
        /// <param name="fineIntervals"></param>
        internal void FinalScan(List<UInt64> fineIntervals)
        {
            if (bgw.CancellationPending) return;
            Dictionary<MicrowaveTuning, MicrowaveLinkQuality> tempPeakDict = new Dictionary<MicrowaveTuning, MicrowaveLinkQuality>();

            //For each interval
            for (int i = 0; i < fineIntervals.Count && !bgw.CancellationPending; i += 2)
            {
                UInt64 intervalStart = fineIntervals[i];
                UInt64 intervalEnd = fineIntervals[i + 1];

                Debug.WriteLine("Starting Interval: " + intervalStart + " to: " + intervalEnd);
                UInt64 currentPeak = 0;
                MicrowaveLinkQuality currentPeakStrength = new MicrowaveLinkQuality(0);
                //For each frequency
                for (UInt64 currentFreq = intervalStart;
                    currentFreq < intervalEnd && !bgw.CancellationPending; currentFreq++)
                {
                    _prototype.Frequency = currentFreq;
                    try
                    {
                        rx.SetTuning(_prototype);
                    }
                    catch
                    {
                        continue;
                    }
                    MicrowaveLinkQuality strength = rx.GetLinkQuality();
                    Debug.WriteLine("Freq Changed: " + currentFreq + "\r\nStrength: " + strength);

                    if (strength.ReceivedCarrierLevel > _minStrength)
                    {
                        if (strength.ReceivedCarrierLevel > currentPeakStrength.ReceivedCarrierLevel)
                        {
                            currentPeak = currentFreq;
                            currentPeakStrength = strength;
                        }
                        else//save peak and watch for next one
                        {
                            if ((currentPeak > 0) && (currentPeakStrength.ReceivedCarrierLevel > 0))
                            {
                                tempPeakDict.Add(new MicrowaveTuning(_prototype), new MicrowaveLinkQuality(currentPeakStrength));
                            }
                            Debug.WriteLine("Final Peak: " + currentPeak + " strength: " + currentPeakStrength);
                            currentPeakStrength = strength;
                            currentPeak = 0;
                        }
                    }
                    else
                    {
                        //Save peak
                        if ((currentPeak > 0) && (currentPeakStrength.ReceivedCarrierLevel > 0))
                        {
                            MicrowaveTuning peakTuning = new MicrowaveTuning(_prototype);
                            peakTuning.Frequency = currentPeak;
                            tempPeakDict.Add(peakTuning, new MicrowaveLinkQuality(currentPeakStrength));
                            Debug.WriteLine("Final Peak: " + currentPeak + " strength: " + currentPeakStrength);
                        }
                        currentPeakStrength.ReceivedCarrierLevel = 0;
                    }
                }//End Frequencies in Interval
                if ((currentPeak > 0) && (currentPeakStrength.ReceivedCarrierLevel > 0))
                {
                    MicrowaveTuning peakTuning = new MicrowaveTuning(_prototype);
                    peakTuning.Frequency = currentPeak;
                    tempPeakDict.Add(peakTuning, new MicrowaveLinkQuality(currentPeakStrength));
                    Debug.WriteLine("Final Peak: " + currentPeak + " strength: " + currentPeakStrength);
                }
            }//End Intervals
            if (!bgw.CancellationPending)
                PeakDictionary = tempPeakDict;
        }

        /// <summary>
        /// Scans from last start to last end using ones (e.g. 1700, 1701, 1702), 
        /// putting final results into PeakDictionary.
        /// </summary>
        public void FinalScan()
        {
            List<UInt64> interval = new List<UInt64>();
            interval.Add(_startFreq);
            interval.Add(_endFreq);
            FinalScan(interval);
        }
    }
    /// <summary>
    /// Event encapsulating the results of a scan
    /// </summary>
    public class ScanCompleteEvent : EventArgs
    {
        private Exception _exc;
        /// <summary>
        /// Exception which stopped a scan, if any.
        /// </summary>
        public Exception Error
        {
            get
            {
                return _exc;
            }
            set
            {
                _exc = value;
            }
        }

        private Dictionary<MicrowaveTuning, MicrowaveLinkQuality> _peakDict;
        /// <summary>
        /// Dictionary of frequency to signal strength of the individual
        /// peaks discovered
        /// </summary>
        public Dictionary<MicrowaveTuning, MicrowaveLinkQuality> PeakDictionary
        {
            get
            {
                return _peakDict;
            }
            set
            {
                _peakDict = value;
            }
        }

        /// <summary>
        /// Number of passes used to generate these results.
        /// </summary>
        public int passes = 1;
        private UInt64 _endFreq = 0;
        /// <summary>
        /// The end frequency of the scan performed, inclusive. If this
        /// frequency is -1, indicates that an error stopped the scan at
        /// some unknown point. PeakDictionary is still valid.
        /// </summary>
        public UInt64 EndFrequency
        {
            get
            {
                return _endFreq;
            }
            set
            {
                _endFreq = value;
            }
        }

        private UInt64 _startFreq = 0;
        /// <summary>
        /// The start frequency of the scan performed, inclusive.
        /// </summary>
        public UInt64 StartFrequency
        {
            get
            {
                return _startFreq;
            }
            set
            {
                _startFreq = value;
            }
        }

        private int _minStrength;
        /// <summary>
        /// The minimum signal strength required to register a peak.
        /// </summary>
        public int MinSignalStrength
        {
            get
            {
                return _minStrength;
            }
            set
            {
                _minStrength = value;
            }
        }
    }
}
