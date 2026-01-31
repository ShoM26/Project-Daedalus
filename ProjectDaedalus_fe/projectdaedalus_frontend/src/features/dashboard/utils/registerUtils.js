// This is a pure async function. It knows nothing about React.
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

  return response.json(); // Return data if needed
};