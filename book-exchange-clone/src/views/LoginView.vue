<template>
  <div class="auth-page">
    <div class="auth-card">
      <h2 class="auth-title">Đăng nhập</h2>
      <form @submit.prevent="handleLogin" class="auth-form">
        <div class="form-group">
          <label for="login-email">Email</label>
          <input id="login-email" type="email" v-model="form.email" required class="form-input" />
        </div>
        <div class="form-group">
          <label for="login-password">Mật khẩu</label>
          <input id="login-password" type="password" v-model="form.password" required class="form-input" />
        </div>
        <button type="submit" class="btn btn-primary btn-block">Đăng nhập</button>
      </form>
      <div v-if="error" class="error-message">{{ error }}</div>
      <p class="auth-link">Chưa có tài khoản? <router-link to="/register">Đăng ký ngay</router-link></p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { login } from '../services/api';

const router = useRouter();
const form = ref({
  email: '',
  password: ''
});
const error = ref(null);

const handleLogin = async () => {
  try {
    error.value = null;
    const response = await login(form.value);
    localStorage.setItem('token', response.data.token);
    localStorage.setItem('user', JSON.stringify(response.data));
    if (response.data && response.data.isAdmin) {
      window.location.href = '/admin';
    } else {
      window.location.href = '/';
    }
  } catch (err) {
    error.value = err.response?.data?.message || 'Có lỗi xảy ra khi đăng nhập';
  }
};
</script>

<style scoped>
.auth-page {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: calc(100vh - 140px);
  padding: 40px 20px;
}

.auth-card {
  background-color: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05), 0 2px 4px -1px rgba(0, 0, 0, 0.03);
  width: 100%;
  max-width: 400px;
  padding: 32px 40px;
}

.auth-title {
  font-size: 24px;
  font-weight: 700;
  color: #111827;
  text-align: center;
  margin: 0 0 24px 0;
}

.auth-form {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  font-size: 14px;
  font-weight: 600;
  color: #374151;
  margin-bottom: 8px;
}

.form-input {
  width: 100%;
  height: 42px;
  padding: 0 12px;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 15px;
  font-family: inherit;
  color: #111827;
  transition: border-color 0.15s, box-shadow 0.15s;
}

.form-input:focus {
  outline: none;
  border-color: #1e40af;
  box-shadow: 0 0 0 3px rgba(30, 64, 175, 0.1);
}

.btn {
  font-size: 15px;
  font-weight: 600;
  padding: 10px 16px;
  border-radius: 8px;
  text-align: center;
  cursor: pointer;
  border: none;
  font-family: inherit;
  transition: background-color 0.15s;
  margin-top: 8px;
}

.btn-primary {
  background-color: #1e40af;
  color: #ffffff;
}

.btn-primary:hover {
  background-color: #1e3a8a;
}

.btn-block {
  width: 100%;
  display: block;
}

.error-message {
  margin-top: 16px;
  padding: 12px;
  background-color: #fef2f2;
  color: #dc2626;
  font-size: 14px;
  border-radius: 8px;
  text-align: center;
}

.auth-link {
  margin-top: 24px;
  text-align: center;
  font-size: 14px;
  color: #6b7280;
}

.auth-link a {
  color: #1e40af;
  font-weight: 600;
  text-decoration: none;
}

.auth-link a:hover {
  text-decoration: underline;
}
</style>
