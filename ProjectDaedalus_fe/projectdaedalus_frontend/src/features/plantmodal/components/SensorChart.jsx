import React, { useState, useEffect } from 'react';
import { plantService } from '../services/plantService';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, ReferenceLine } from 'recharts';
import '../styles/SensorChart.css';

function SensorChart({ deviceId, moistureMin, moistureMax }) {
  const [timeRange, setTimeRange] = useState('24h');
  const [readings, setReadings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  if (!deviceId) {
    return <div>Error: No device ID provided</div>;
  }

  const getDateRange = (range) => {
    const endDate = new Date(); 
    const startDate = new Date();

    switch(range) {
      case '4h':
        startDate.setHours(startDate.getHours() - 4);
        break;
      case '24h':
        startDate.setHours(startDate.getHours() - 24);
        break;
      case '7d':
        startDate.setDate(startDate.getDate() - 7);
        break;
      default:
        startDate.setHours(startDate.getHours() - 24);
    }

    return {
      startDate: startDate.toISOString(),
      endDate: endDate.toISOString()
    };
  };

  useEffect(() => {
    const fetchReadings = async () => {
      setLoading(true);
      setError(null);

      try {
        const { startDate, endDate } = getDateRange(timeRange);
        const data = await plantService.getReadingsRange(deviceId, startDate, endDate);
        if(data === null){
          setReadings([]);
        }
        else{
          const formattedData = data.map(reading => ({
        timestamp: new Date(reading.timestamp).toLocaleString('en-US',{
            month: 'short',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        }),
        moisture: reading.moistureLevel,
        date: new Date(reading.timestamp)
      }));
      setReadings(formattedData);
        }
        
        
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchReadings();
  }, [deviceId, timeRange]);

  const handleTimeRangeChange = (e) => {
    setTimeRange(e.target.value);
  };

  const CustomTooltip = ({ active, payload }) => {
    if (active && payload && payload.length) {
      const data = payload[0];
      const isHealthy = data.value >= moistureMin && data.value <= moistureMax;
      return (
        <div className="custom-tooltip">
          <p className="tooltip-time">{data.payload.timestamp}</p>
          <p className={`tooltip-value ${isHealthy ? 'healthy' : 'unhealthy'}`}>
            Moisture: {data.value}%
          </p>
          {!isHealthy && (
            <p className="tooltip-warning">
              {data.value < moistureMin ? 'Too dry!' : 'Too wet!'}
            </p>
          )}
        </div>
      );
    }
    return null;
  };


  return (
    <div className="sensor-chart">
      <div className="chart-header">
        <h3>Moisture History</h3>
        <div className="time-range-selector">
          <select 
            id="timeRange"
            value={timeRange} 
            onChange={handleTimeRangeChange}
          >
            <option value="4h">Last 4 Hours</option>
            <option value="24h">Last 24 Hours</option>
            <option value="7d">Last 7 Days</option>
          </select>
        </div>
      </div>

      {loading ? (
        <div className="chart-state">
          <div className="loading-spinner"></div>
          <p>Loading sensor data...</p>
        </div>
      ) : error ? (
        <div className="chart-state error">
          <p>Error loading data: {error}</p>
        </div>
      ) : readings.length === 0 ? (
        <div className="chart-state no-data">
          <p>No sensor data available for this time range.</p>
          <p className="hint">Try selecting a longer time range.</p>
        </div>
      ) : (
        <div className="chart-container">
          <ResponsiveContainer width="100%" height={350}>
            <LineChart data={readings} margin={{ top: 10, right: 30, left: 10, bottom: 60 }}>
              <defs>
                <linearGradient id="moistureGradient" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="5%" stopColor="#4CAF50" stopOpacity={0.8}/>
                  <stop offset="95%" stopColor="#4CAF50" stopOpacity={0.1}/>
                </linearGradient>
              </defs>
              
              <CartesianGrid strokeDasharray="3 3" stroke="#e0e0e0" />
              
              <ReferenceLine 
                y={moistureMax} 
                stroke="#FF9800" 
                strokeDasharray="5 5"
                label={{ value: 'Max', position: 'right', fill: '#FF9800' }}
              />
              <ReferenceLine 
                y={moistureMin} 
                stroke="#FF9800" 
                strokeDasharray="5 5"
                label={{ value: 'Min', position: 'right', fill: '#FF9800' }}
              />
              
              <XAxis 
                dataKey="timestamp" 
                angle={-45}
                textAnchor="end"
                height={100}
                tick={{ fontSize: 11, fill: '#666' }}
                stroke="#999"
              />
              <YAxis 
                label={{ 
                  value: 'Moisture Level (%)', 
                  angle: -90, 
                  position: 'insideLeft',
                  style: { fill: '#666' }
                }}
                tick={{ fill: '#666' }}
                stroke="#999"
              />
              
              <Tooltip content={<CustomTooltip />} />
              
              <Legend 
                wrapperStyle={{ paddingTop: '20px' }}
                iconType="line"
              />
              
              <Line 
                type="monotone" 
                dataKey="moisture" 
                stroke="#4CAF50" 
                strokeWidth={3}
                name="Moisture Level"
                dot={{ r: 4, fill: '#4CAF50', strokeWidth: 2, stroke: '#fff' }}
                activeDot={{ r: 6, fill: '#4CAF50', strokeWidth: 3, stroke: '#fff' }}
                fill="url(#moistureGradient)"
              />
            </LineChart>
          </ResponsiveContainer>
        </div>
      )}
    </div>
  );
}

export default SensorChart;