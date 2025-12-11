// This is a pure async function. It knows nothing about React.
export const configureBridge = async (userToken, apiBaseUrl) => {
  const response = await fetch('http://localhost:5050/', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ 
        UserToken: userToken,
        ApiBaseUrl: apiBaseUrl 
    })
  });

  if (!response.ok) {
    throw new Error(`Bridge responded with status: ${response.status}`);
  }

  return response.json(); // Return data if needed
};