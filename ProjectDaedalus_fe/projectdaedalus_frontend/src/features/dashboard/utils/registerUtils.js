export const configureBridge = async (userToken, apiBaseUrl) => {
  const response = await fetch('http://localhost:5000/setup', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ 
        Token: userToken,
    })
  });

  if (!response.ok) {
    throw new Error(`Bridge responded with status: ${response.status}`);
  }

  return response.json();
};