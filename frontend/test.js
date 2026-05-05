const registerData = {
  email: `test_${Date.now()}@example.com`,
  username: `testuser_${Date.now()}`,
  password: 'StrongP@ssword123!',
  confirmPassword: 'StrongP@ssword123!',
  firstName: 'Test',
  lastName: 'User',
  dateOfBirth: '2000-01-01',
  gender: 'NotSpecified',
  fitnessGoal: 'NotSpecified'
};

async function testAuthFlow() {
  console.log('Testing Registration...');
  const regRes = await fetch('http://localhost:5001/api/v1/auth/register', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(registerData)
  });
  
  if (!regRes.ok) {
    const error = await regRes.text();
    console.error('Registration failed:', error);
    return;
  }
  console.log('Registration successful!');

  console.log('Testing Login...');
  const loginRes = await fetch('http://localhost:5001/api/v1/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email: registerData.email, password: registerData.password })
  });

  if (!loginRes.ok) {
    const error = await loginRes.text();
    console.error('Login failed:', error);
    return;
  }
  
  const loginData = await loginRes.json();
  console.log('Login successful! Access token obtained.');
  
  console.log('Testing Profile Fetch...');
  const profileRes = await fetch('http://localhost:5001/api/v1/account/profile', {
    headers: { 'Authorization': `Bearer ${loginData.accessToken}` }
  });

  if (!profileRes.ok) {
    console.error('Profile fetch failed:', profileRes.status);
    return;
  }
  const profileData = await profileRes.json();
  console.log('Profile fetched successfully:', profileData.firstName, profileData.lastName);
  
  console.log('All tests passed successfully! ✅');
}

testAuthFlow();
