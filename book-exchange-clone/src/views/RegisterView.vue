<template>
  <div class="auth-page">
    <div class="auth-card">
      <h2 class="auth-title">Đăng ký tài khoản</h2>
      <form @submit.prevent="handleRegister" class="auth-form">
        <div class="form-group">
          <label for="reg-fullname">Họ tên</label>
          <input id="reg-fullname" type="text" v-model="form.fullName" required class="form-input" />
        </div>
        <div class="form-group">
          <label for="reg-email">Email</label>
          <input id="reg-email" type="email" v-model="form.email" required class="form-input" />
        </div>
        <div class="form-group">
          <label for="reg-password">Mật khẩu</label>
          <input id="reg-password" type="password" v-model="form.password" required class="form-input" />
        </div>
        <div class="form-group">
          <label for="reg-phone">Điện thoại</label>
          <input id="reg-phone" type="text" v-model="form.phone" class="form-input" />
        </div>
        <button type="submit" class="btn btn-primary btn-block">Đăng ký</button>
      </form>
      <div v-if="error" class="error-message">{{ error }}</div>
      <p class="auth-link">Đã có tài khoản? <router-link to="/login">Đăng nhập</router-link></p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { register } from '../services/api';
import { useToast } from '../composables/useToast';

const { showToast } = useToast();
const router = useRouter();
const form = ref({
  fullName: '',
  email: '',
  password: '',
  phone: ''
});
const error = ref(null);

const handleRegister = async () => {
  try {
    error.value = null;
    await register(form.value);
    showToast('Đăng ký thành công! Đang chuyển hướng đến đăng nhập.', 'success');
    router.push('/login');
  } catch (err) {
    error.value = err.response?.data?.message || 'Có lỗi xảy ra khi đăng ký.';
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
