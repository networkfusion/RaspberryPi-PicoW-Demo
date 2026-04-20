using System;
using System.Device.Adc;

namespace nanoFramework.Hardware.PiPico
{
    public class OnboardAdcDevice : IDisposable
    {
        private bool _disposed;
        private AdcController adcController = new();
        private AdcChannel adcInternalVsysChannel;
        private AdcChannel adcInternalTempChannel;
        private static readonly int ADC_VSYS_CHANNEL = 3;
        private static readonly int ADC_TEMP_CHANNEL = 4;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetInternalVoltage()
        {
            adcController ??= new AdcController();
            adcInternalVsysChannel ??= adcController.OpenChannel(ADC_VSYS_CHANNEL);
            var raw = adcInternalVsysChannel.ReadValue();
            return (raw / 4095.0) * 3.3 * 3;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetInternalTemperature()
        {
            adcController ??= new AdcController();
            adcInternalTempChannel ??= adcController.OpenChannel(ADC_TEMP_CHANNEL);

            // Get raw 16-bit value and convert to voltage
            var raw = adcInternalTempChannel.ReadValue();

            // the built-in sensor has a typical voltage of 0.706 at 27c, with a slope -1.721mV of per degree Celsius
            // Formula from datasheet: 27 - (voltage - 0.706) / 0.001721
            return 27 - (raw * 3.3 / 4096 - 0.706) / 0.001721; // TODO: should possibly be using the VSYS as a ref?



        }


        /// <summary>
        /// Releases unmanaged resources
        /// and optionally release managed resources
        /// </summary>
        /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            adcInternalVsysChannel.Dispose();
            adcInternalTempChannel.Dispose();
            adcController = null;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Dispose(true);
                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }

    }
}
