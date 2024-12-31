import React, { useState, useEffect, useRef } from "react";
import {
  View,
  Text,
  TextInput,
  Button,
  StyleSheet,
  ActivityIndicator,
  Animated,
} from "react-native";

export default function App() {
  const [location, setLocation] = useState("");
  const [weatherData, setWeatherData] = useState<any>(null);
  const [loading, setLoading] = useState(false);

  const backgroundColor = useRef(new Animated.Value(0)).current;

  const fetchWeather = async () => {
    setLoading(true);
    setWeatherData(null); 
    try {
      const response = await fetch(
        `http://10.0.2.2:5108/api/Weather/${location}`
      );
      if (!response.ok) {
        throw new Error("Error fetching weather data");
      }
      const data = await response.json();
      setWeatherData(data);
      animateBackground(data.main.temp); 
    } catch (error) {
      console.error(error);
      alert("Could not fetch weather data. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  const animateBackground = (temperature: number) => {
    let targetValue = 0; 

    if (temperature >= 25) {
      targetValue = 2; 
    } else if (temperature >= 15) {
      targetValue = 1; 
    }

    Animated.timing(backgroundColor, {
      toValue: targetValue,
      duration: 1000,
      useNativeDriver: false,
    }).start();
  };

  const backgroundInterpolation = backgroundColor.interpolate({
    inputRange: [0, 1, 2],
    outputRange: ["#1e3a8a", "#60a5fa", "#f97316"], 
  });

  return (
    <Animated.View style={[styles.container, { backgroundColor: backgroundInterpolation }]}>
      {!weatherData && (
        <>
          <Text style={styles.title}>Aplicación del tiempo</Text>
          <TextInput
            style={styles.input}
            placeholder="Ingresa el nombre de una ciudad"
            value={location}
            onChangeText={setLocation}
          />
          <Button title="Busca el tiempo actual" onPress={fetchWeather} />
          {loading && <ActivityIndicator size="large" color="#ffffff" />}
        </>
      )}
      {weatherData && (
        <View style={styles.resultContainer}>
          <Text style={styles.cityName}>{weatherData.name}</Text>
          <Text style={styles.temperature}>
            {Math.round(weatherData.main.temp)}°C
          </Text>
          <Text style={styles.description}>
            {weatherData.weather[0].description}
          </Text>
          <Text style={styles.wind}>
            💨 Wind: {Math.round(weatherData.wind.speed)} m/s,{" "}
            {getWindDirection(weatherData.wind.deg)}
          </Text>
          <Button title="Busca otra vez" onPress={() => setWeatherData(null)} />
        </View>
      )}
    </Animated.View>
  );
}

const getWindDirection = (degree: number): string => {
  if (degree > 337.5 || degree <= 22.5) return "N";
  if (degree > 22.5 && degree <= 67.5) return "NE";
  if (degree > 67.5 && degree <= 112.5) return "E";
  if (degree > 112.5 && degree <= 157.5) return "SE";
  if (degree > 157.5 && degree <= 202.5) return "S";
  if (degree > 202.5 && degree <= 247.5) return "SW";
  if (degree > 247.5 && degree <= 292.5) return "W";
  if (degree > 292.5 && degree <= 337.5) return "NW";
  return "N";
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    padding: 20,
  },
  title: {
    fontSize: 24,
    fontWeight: "bold",
    marginBottom: 20,
    color: "#ffffff",
  },
  input: {
    borderWidth: 1,
    borderColor: "#ccc",
    borderRadius: 8,
    padding: 10,
    width: "100%",
    marginBottom: 20,
    backgroundColor: "#fff",
  },
  resultContainer: {
    alignItems: "center",
    backgroundColor: "rgba(255, 255, 255, 0.9)",
    padding: 20,
    borderRadius: 10,
    shadowColor: "#000",
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.2,
    shadowRadius: 4,
    elevation: 5,
    width: "100%",
  },
  cityName: {
    fontSize: 28,
    fontWeight: "bold",
    color: "#333",
  },
  temperature: {
    fontSize: 50,
    fontWeight: "bold",
    color: "#ff5733",
    marginVertical: 10,
  },
  description: {
    fontSize: 18,
    fontStyle: "italic",
    color: "#666",
    marginBottom: 20,
  },
  wind: {
    fontSize: 16,
    color: "#555",
    marginBottom: 20,
  },
});
