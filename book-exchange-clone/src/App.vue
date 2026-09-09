<template>
  <div>
    <nav class="navbar" v-if="!isAdminRoute">
      <router-link to="/" class="nav-brand">
        <svg class="nav-brand-icon" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M4 19.5A2.5 2.5 0 016.5 17H20" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M6.5 2H20v20H6.5A2.5 2.5 0 014 19.5v-15A2.5 2.5 0 016.5 2z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M8 7h8M8 11h6" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
        </svg>
        <span>BookExchange</span>
      </router-link>
      <div class="nav-links">
        <template v-if="!isLoggedIn">
          <router-link to="/login" class="nav-btn nav-btn-ghost">Đăng nhập</router-link>
          <router-link to="/register" class="nav-btn nav-btn-solid">Đăng ký</router-link>
        </template>
        <template v-else>
          <router-link v-if="user?.isAdmin" to="/admin" class="nav-admin-link">
            <span class="nav-admin-icon">🛡️</span>
            <span>Quản trị hệ thống</span>
          </router-link>
          <router-link to="/schedule" class="nav-link">Lịch của tôi</router-link>
          <router-link to="/meetings" class="nav-link">Quản lý lịch hẹn</router-link>
          <router-link to="/profile" class="nav-link">Hồ sơ</router-link>
          <button @click="logout" class="nav-btn-text">Đăng xuất</button>
        </template>
      </div>
    </nav>
    <main :class="isAdminRoute ? 'admin-layout-wrapper' : 'main-container'">
      <RouterView />
    </main>
    <ToastContainer />
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import { RouterView, useRoute } from 'vue-router';
import ToastContainer from './components/ToastContainer.vue';

const route = useRoute();
const isAdminRoute = computed(() => route.path.startsWith('/admin'));

const isLoggedIn = ref(!!localStorage.getItem('token'));
const user = ref(JSON.parse(localStorage.getItem('user') || '{}'));

const logout = () => {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
  isLoggedIn.value = false;
  user.value = {};
  window.location.href = '/';
};
</script>

<style>
*,
*::before,
*::after {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

body {
  font-family: 'Segoe UI', -apple-system, BlinkMacSystemFont, Arial, sans-serif;
  color: #111827;
  background-color: #f9fafb;
  -webkit-font-smoothing: antialiased;
}

.navbar {
  background-color: #ffffff;
  border-bottom: 1px solid #e5e7eb;
  padding: 0 32px;
  height: 60px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  position: sticky;
  top: 0;
  z-index: 100;
}

.nav-brand {
  display: flex;
  align-items: center;
  gap: 8px;
  text-decoration: none;
  color: #1e40af;
  font-size: 20px;
  font-weight: 700;
}

.nav-brand-icon {
  width: 24px;
  height: 24px;
}

.nav-links {
  display: flex;
  align-items: center;
  gap: 8px;
}

.nav-link {
  color: #374151;
  text-decoration: none;
  font-size: 14px;
  font-weight: 500;
  padding: 6px 12px;
  border-radius: 6px;
  transition: color 0.15s, background-color 0.15s;
}

.nav-link:hover {
  color: #1e40af;
  background-color: #eff6ff;
}

.nav-btn {
  font-size: 14px;
  font-weight: 600;
  padding: 8px 18px;
  border-radius: 8px;
  text-decoration: none;
  cursor: pointer;
  transition: all 0.15s;
  border: none;
}

.nav-btn-ghost {
  color: #1e40af;
  border: 1.5px solid #1e40af;
  background: transparent;
}

.nav-btn-ghost:hover {
  background-color: #eff6ff;
}

.nav-btn-solid {
  color: #ffffff;
  background-color: #1e40af;
}

.nav-btn-solid:hover {
  background-color: #1e3a8a;
}

.nav-admin-link {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  background-color: #eff6ff;
  color: #1d4ed8;
  border: 1px solid #bfdbfe;
  font-size: 13px;
  font-weight: 600;
  padding: 6px 14px;
  border-radius: 8px;
  text-decoration: none;
  transition: all 0.15s ease;
  margin-right: 6px;
}

.nav-admin-link:hover {
  background-color: #dbeafe;
  color: #1e40af;
  border-color: #93c5fd;
  transform: translateY(-1px);
}

.nav-admin-icon {
  font-size: 14px;
}

.nav-user {
  font-size: 14px;
  font-weight: 600;
  color: #374151;
  padding: 0 8px;
}

.nav-btn-text {
  background: none;
  border: none;
  color: #6b7280;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  padding: 6px 12px;
  border-radius: 6px;
  font-family: inherit;
  transition: color 0.15s, background-color 0.15s;
}

.nav-btn-text:hover {
  color: #dc2626;
  background-color: #fef2f2;
}

.main-container {
  padding: 32px 32px;
  max-width: 1200px;
  margin: 0 auto;
}

.admin-layout-wrapper {
  padding: 0;
  margin: 0;
  max-width: none;
  width: 100%;
}

@media (max-width: 768px) {
  .navbar {
    padding: 0 16px;
  }
  .nav-brand span {
    font-size: 17px;
  }
  .nav-links {
    gap: 4px;
  }
  .nav-btn {
    padding: 6px 12px;
    font-size: 13px;
  }
  .main-container {
    padding: 20px 16px;
  }
}
</style>
