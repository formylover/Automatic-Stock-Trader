﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using Stonks2.Configuration;

namespace Stonks2
{
    public class ModelBuilder
    {
        private MLConfig _config;

        public ModelBuilder(MLConfig config)
        {
            _config = config;
        }

        public TimeSeriesPredictionEngine<ModelInput, ModelOutput> BuildModel(IList<ModelInput> inputs)
        {
            //Create model
            var mlContext = new MLContext();
            var trainDataVeiw = mlContext.Data.LoadFromEnumerable(inputs);

            var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedPriceDiffrence",
                inputColumnName: "PriceDiffrence",
                windowSize: _config.Window_Size,
                seriesLength: _config.Series_Length,
                trainSize: inputs.Count,
                horizon: _config.Horizon,
                confidenceLevel: _config.Confidence_Level,
                confidenceLowerBoundColumn: "LowerBoundPriceDiffrence",
                confidenceUpperBoundColumn: "UpperBoundPriceDiffrence");

            var forecaster = forecastingPipeline.Fit(trainDataVeiw);
            return forecaster.CreateTimeSeriesEngine<ModelInput, ModelOutput>(mlContext);
        }
    }
}