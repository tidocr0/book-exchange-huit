<template>
  <div class="profile-page">
    <div v-if="loadingProfile" class="loading-container">
      <div class="spinner"></div>
      <p>Đang tải thông tin hồ sơ...</p>
    </div>

    <div v-else class="profile-layout">
      <div class="profile-header-card">
        <div class="header-left">
          <div class="avatar-wrapper" @click="triggerAvatarUpload" title="Bấm để đổi ảnh đại diện">
            <input
              ref="avatarInput"
              type="file"
              accept=".jpg,.jpeg,.png"
              class="hidden-file-input"
              @change="handleAvatarChange"
            />
            <img
              v-if="profile.avatarUrl"
              :src="'http://localhost:5100' + profile.avatarUrl"
              alt="Avatar"
              class="avatar-image"
            />
            <div v-else class="avatar-circle">
              {{ getInitials(profile.fullName) }}
            </div>
            <div class="avatar-hover-overlay">
              <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M23 19a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h4l2-3h6l2 3h4a2 2 0 0 1 2 2z" />
                <circle cx="12" cy="13" r="4" />
              </svg>
            </div>
          </div>

          <div class="header-name-row">
            <h2 class="profile-name">{{ profile.fullName }}</h2>
            <span
              v-if="profile.verificationStatus === 2 || profile.isVerified"
              class="verify-badge verified"
              title="Tài khoản sinh viên đã xác thực"
            >
              <svg width="14" height="14" viewBox="0 0 20 20" fill="currentColor">
                <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
              </svg>
            </span>
            <span
              v-else
              class="verify-badge unverified"
              :title="profile.verificationStatus === 1 ? 'Đang chờ Admin duyệt xác thực' : 'Tài khoản chưa xác thực'"
            >
              <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                <line x1="18" y1="6" x2="6" y2="18" />
                <line x1="6" y1="6" x2="18" y2="18" />
              </svg>
            </span>
          </div>
        </div>

        <div class="header-stats">
          <div class="stat-pill">
            <span class="stat-icon">🤝</span>
            <span>Đã bán: {{ soldCount }} cuốn sách</span>
          </div>
          <div class="stat-pill">
            <span class="stat-icon">📅</span>
            <span>Tham gia từ: {{ formatDate(profile.createdAt) }}</span>
          </div>
        </div>
      </div>

      <div v-if="user.isAdmin" class="admin-access-banner">
        <div class="admin-banner-content">
          <div class="admin-banner-badge">Quản Trị</div>
          <div class="admin-banner-text">
            <strong>Bảng Điều Khiển Quản Trị Hệ Thống</strong>
            <span>Xem thống kê, duyệt xác thực thẻ sinh viên và xử lý báo cáo vi phạm.</span>
          </div>
        </div>
        <router-link to="/admin" class="btn-admin-link">
          Truy cập Trang Quản Trị →
        </router-link>
      </div>

      <div class="profile-grid" :class="{ 'single-col': profile.verificationStatus === 2 }">
        <div class="section-card personal-info-section">
          <div class="section-header">
            <svg class="section-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M20 21v-2a4 4 0 00-4-4H8a4 4 0 00-4 4v2" />
              <circle cx="12" cy="7" r="4" />
            </svg>
            <h3>Thông tin cá nhân</h3>
          </div>

          <form @submit.prevent="handleUpdateProfile" class="profile-form">
            <div class="form-group">
              <label for="fullName">Họ và tên</label>
              <input
                id="fullName"
                type="text"
                v-model="editForm.fullName"
                required
                class="form-input"
                placeholder="Nhập họ và tên"
              />
            </div>

            <div class="form-group">
              <label for="email">Email</label>
              <input
                id="email"
                type="text"
                :value="profile.email"
                disabled
                class="form-input form-input-disabled"
              />
              <span class="field-hint">Email dùng để đăng nhập và không thể thay đổi.</span>
            </div>

            <div class="form-group">
              <label for="phone">Số điện thoại</label>
              <input
                id="phone"
                type="text"
                v-model="editForm.phone"
                class="form-input"
                placeholder="Nhập số điện thoại liên hệ"
              />
            </div>

            <div class="form-actions">
              <button type="submit" class="btn btn-primary" :disabled="savingProfile">
                <span v-if="savingProfile">Đang lưu...</span>
                <span v-else>Lưu thay đổi</span>
              </button>
            </div>
          </form>
        </div>

        <div v-if="profile.verificationStatus !== 2" class="section-card verification-section">
          <div class="section-header">
            <svg class="section-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
            </svg>
            <h3>Xác thực tài khoản</h3>
          </div>

          <div v-if="profile.verificationStatus === 1" class="verification-status-banner banner-pending">
            <div class="status-icon-box">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="10" />
                <polyline points="12 6 12 12 16 14" />
              </svg>
            </div>
            <div>
              <h4>Đang chờ xét duyệt</h4>
              <p>Yêu cầu xác thực thẻ sinh viên của bạn đang chờ Admin duyệt.</p>
            </div>
          </div>

          <div v-else class="verification-form-wrapper">
            <div v-if="profile.verificationStatus === 3" class="verification-status-banner banner-rejected">
              <div class="status-icon-box">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <circle cx="12" cy="12" r="10" />
                  <line x1="15" y1="9" x2="9" y2="15" />
                  <line x1="9" y1="9" x2="15" y2="15" />
                </svg>
              </div>
              <div>
                <h4 class="text-rejected">Yêu cầu trước đã bị từ chối, vui lòng gửi lại</h4>
                <p>Ảnh chụp thẻ chưa rõ ràng hoặc không hợp lệ. Vui lòng chụp rõ thông tin thẻ sinh viên.</p>
              </div>
            </div>

            <p class="verification-desc">
              Tải lên ảnh thẻ sinh viên của bạn (mặt trước có MSSV & Họ tên) để nhận huy hiệu xác thực.
            </p>

            <form @submit.prevent="handleVerificationSubmit" class="verification-upload-form">
              <div class="upload-box single-upload">
                <div class="file-drop-area" :class="{ 'has-file': cardPreview }" @click="$refs.cardInput.click()">
                  <input
                    ref="cardInput"
                    type="file"
                    accept=".jpg,.jpeg,.png"
                    class="hidden-file-input"
                    @change="onCardFileChange"
                  />
                  <img v-if="cardPreview" :src="cardPreview" alt="Thẻ sinh viên" class="upload-preview" />
                  <div v-else class="upload-placeholder">
                    <svg width="36" height="36" viewBox="0 0 24 24" fill="none" stroke="#9ca3af" stroke-width="1.5">
                      <rect x="3" y="3" width="18" height="18" rx="2" ry="2" />
                      <circle cx="8.5" cy="8.5" r="1.5" />
                      <polyline points="21 15 16 10 5 21" />
                    </svg>
                    <span class="upload-title">Bấm để tải ảnh thẻ sinh viên</span>
                    <small>Mặt trước có MSSV & Họ tên (JPG, PNG tối đa 5MB)</small>
                  </div>
                </div>
              </div>

              <div class="form-actions">
                <button
                  type="submit"
                  class="btn btn-primary"
                  :disabled="submittingVerification || !cardFile"
                >
                  <span v-if="submittingVerification">Đang gửi yêu cầu...</span>
                  <span v-else>Gửi xác thực</span>
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>

      <div class="section-card my-books-section">
        <div class="section-header space-between">
          <div class="header-left-title">
            <svg class="section-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M4 19.5A2.5 2.5 0 016.5 17H20" />
              <path d="M6.5 2H20v20H6.5A2.5 2.5 0 014 19.5v-15A2.5 2.5 0 016.5 2z" />
            </svg>
            <h3>Tủ sách của tôi</h3>
            <span class="books-count" v-if="myListings.length > 0">({{ myListings.length }})</span>
          </div>
          <router-link to="/create" class="btn btn-outline-sm">
            <svg width="14" height="14" viewBox="0 0 16 16" fill="none">
              <path d="M8 3v10M3 8h10" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
            </svg>
            Đăng tin mới
          </router-link>
        </div>

        <div v-if="loadingListings" class="sub-loading">
          <div class="spinner"></div>
          <p>Đang tải danh sách sách...</p>
        </div>

        <div v-else-if="myListings.length === 0" class="empty-books-state">
          <svg class="empty-icon" viewBox="0 0 80 80" fill="none">
            <rect x="10" y="8" width="28" height="40" rx="3" stroke="#d1d5db" stroke-width="2" fill="#f3f4f6" />
            <rect x="42" y="18" width="28" height="40" rx="3" stroke="#d1d5db" stroke-width="2" fill="#f9fafb" />
          </svg>
          <p class="empty-title">Bạn chưa đăng bán quyển sách nào</p>
          <p class="empty-hint">Hãy chia sẻ những cuốn sách bạn không còn dùng tới cho các bạn sinh viên khác!</p>
          <router-link to="/create" class="btn btn-primary mt-3">Đăng tin ngay</router-link>
        </div>

        <div v-else class="my-books-grid">
          <router-link
            v-for="item in myListings"
            :key="item.listingId"
            :to="'/listing/' + item.listingId"
            class="book-card"
          >
            <div class="book-card-img">
              <img
                v-if="item.images && item.images.length > 0"
                :src="formatImageUrl(item.images[0].imageUrl)"
                alt="Ảnh sách"
                @error="onImgError"
              />
              <div v-else class="book-card-noimg">
                <svg width="32" height="32" viewBox="0 0 24 24" fill="none">
                  <path d="M4 19.5A2.5 2.5 0 016.5 17H20" stroke="#d1d5db" stroke-width="1.5" stroke-linecap="round" />
                  <path d="M6.5 2H20v20H6.5A2.5 2.5 0 014 19.5v-15A2.5 2.5 0 016.5 2z" stroke="#d1d5db" stroke-width="1.5" stroke-linecap="round" />
                </svg>
              </div>
              <span class="book-card-badge">{{ getConditionText(item.condition) }}</span>
              <span v-if="item.status === 1" class="book-card-status sold">Đã bán</span>
              <span v-if="item.status === 2" class="book-card-status removed">Đã gỡ</span>
            </div>
            <div class="book-card-body">
              <h4 class="book-card-title">{{ item.title }}</h4>
              <p class="book-card-price">{{ item.price.toLocaleString('vi-VN') }} đ</p>
              <div class="book-card-footer">
                <span class="status-indicator" :class="'status-' + item.status">
                  {{ getStatusText(item.status) }}
                </span>
                <span class="date-text">{{ formatDate(item.createdAt) }}</span>
              </div>
            </div>
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { getProfile, updateProfile, uploadAvatar, submitVerification, getListings } from '../services/api';
import { useToast } from '../composables/useToast';

const router = useRouter();
const { showToast } = useToast();

const user = ref(JSON.parse(localStorage.getItem('user') || '{}'));

const loadingProfile = ref(true);
const savingProfile = ref(false);
const submittingVerification = ref(false);
const loadingListings = ref(false);

const profile = ref({
  userId: 0,
  fullName: '',
  email: '',
  phone: '',
  avatarUrl: null,
  isVerified: false,
  verificationStatus: 0,
  createdAt: ''
});

const editForm = reactive({
  fullName: '',
  phone: ''
});

const myListings = ref([]);

const avatarInput = ref(null);
const cardFile = ref(null);
const cardPreview = ref(null);

const checkAuth = () => {
  const token = localStorage.getItem('token');
  if (!token) {
    router.replace('/login');
    return false;
  }
  return true;
};

const fetchProfileData = async () => {
  if (!checkAuth()) return;
  loadingProfile.value = true;
  try {
    const res = await getProfile();
    profile.value = res.data;
    editForm.fullName = res.data.fullName || '';
    editForm.phone = res.data.phone || '';

    await fetchMyListings(res.data.userId);
  } catch (err) {
    showToast(err.response?.data?.message || 'Không thể tải thông tin hồ sơ.', 'error');
  } finally {
    loadingProfile.value = false;
  }
};

const fetchMyListings = async (sellerId) => {
  loadingListings.value = true;
  try {
    const res = await getListings({ sellerId });
    myListings.value = res.data || [];
  } catch (err) {
    console.error(err);
  } finally {
    loadingListings.value = false;
  }
};

const triggerAvatarUpload = () => {
  if (avatarInput.value) {
    avatarInput.value.click();
  }
};

const handleAvatarChange = async (e) => {
  const file = e.target.files[0];
  if (!file) return;

  if (file.size > 5 * 1024 * 1024) {
    showToast('Dung lượng ảnh tối đa 5MB.', 'error');
    return;
  }

  try {
    const formData = new FormData();
    formData.append('avatar', file);

    const res = await uploadAvatar(formData);
    profile.value.avatarUrl = res.data.avatarUrl;
    showToast('Cập nhật ảnh đại diện thành công!', 'success');
  } catch (err) {
    showToast(err.response?.data?.message || 'Không thể tải ảnh đại diện lên.', 'error');
  }
};

const handleUpdateProfile = async () => {
  if (!editForm.fullName.trim()) {
    showToast('Họ và tên không được để trống.', 'error');
    return;
  }

  savingProfile.value = true;
  try {
    const res = await updateProfile({
      fullName: editForm.fullName.trim(),
      phone: editForm.phone.trim() || null
    });
    profile.value = res.data;
    editForm.fullName = res.data.fullName;
    editForm.phone = res.data.phone || '';

    const storedUser = JSON.parse(localStorage.getItem('user') || '{}');
    storedUser.fullName = res.data.fullName;
    localStorage.setItem('user', JSON.stringify(storedUser));

    showToast('Cập nhật thông tin thành công!', 'success');
  } catch (err) {
    showToast(err.response?.data?.message || 'Có lỗi xảy ra khi lưu thông tin.', 'error');
  } finally {
    savingProfile.value = false;
  }
};

const onCardFileChange = (e) => {
  const file = e.target.files[0];
  if (!file) return;

  if (file.size > 5 * 1024 * 1024) {
    showToast('Dung lượng ảnh tối đa 5MB.', 'error');
    return;
  }

  cardFile.value = file;
  cardPreview.value = URL.createObjectURL(file);
};

const handleVerificationSubmit = async () => {
  if (!cardFile.value) {
    showToast('Vui lòng tải lên ảnh thẻ sinh viên.', 'error');
    return;
  }

  submittingVerification.value = true;
  try {
    const formData = new FormData();
    formData.append('CardImage', cardFile.value);

    const res = await submitVerification(formData);
    profile.value.verificationStatus = res.data.verificationStatus;
    cardFile.value = null;
    cardPreview.value = null;

    showToast(res.data.message || 'Gửi yêu cầu xác thực thành công.', 'success');
  } catch (err) {
    showToast(err.response?.data?.message || 'Có lỗi xảy ra khi gửi xác thực.', 'error');
  } finally {
    submittingVerification.value = false;
  }
};

const getInitials = (name) => {
  if (!name) return 'U';
  const parts = name.trim().split(' ');
  if (parts.length >= 2) {
    return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
  }
  return name.slice(0, 2).toUpperCase();
};

const formatDate = (isoStr) => {
  if (!isoStr) return '';
  const d = new Date(isoStr);
  return d.toLocaleDateString('vi-VN');
};

const getConditionText = (c) => {
  switch (c) {
    case 0: return 'Sách gốc - Mới';
    case 1: return 'Sách gốc - Đã qua sử dụng';
    case 2: return 'Sách photo - Mới';
    case 3: return 'Sách photo - Đã qua sử dụng';
    default: return 'Không xác định';
  }
};

const getStatusText = (s) => {
  switch (s) {
    case 0: return 'Đang bán';
    case 1: return 'Đã bán';
    case 2: return 'Đã gỡ';
    default: return '';
  }
};

const soldCount = computed(() => myListings.value.filter(item => item.status === 1).length);

const formatImageUrl = (url) => {
  if (!url) return '';
  if (url.startsWith('http://') || url.startsWith('https://')) return url;
  return 'http://localhost:5100' + url;
};

const onImgError = (e) => {
  e.target.style.display = 'none';
  const fallback = e.target.parentElement.querySelector('.book-card-noimg-fallback');
  if (!fallback) {
    const div = document.createElement('div');
    div.className = 'book-card-noimg book-card-noimg-fallback';
    div.innerHTML = '<svg width="32" height="32" viewBox="0 0 24 24" fill="none"><path d="M4 19.5A2.5 2.5 0 016.5 17H20" stroke="#d1d5db" stroke-width="1.5" stroke-linecap="round"/><path d="M6.5 2H20v20H6.5A2.5 2.5 0 014 19.5v-15A2.5 2.5 0 016.5 2z" stroke="#d1d5db" stroke-width="1.5" stroke-linecap="round"/></svg>';
    e.target.parentElement.insertBefore(div, e.target);
  }
};

onMounted(() => {
  fetchProfileData();
});
</script>

<style scoped>
.profile-page {
  max-width: 1050px;
  margin: 0 auto;
  padding-bottom: 48px;
}

.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 80px 0;
  gap: 16px;
  color: #6b7280;
}

.spinner {
  width: 36px;
  height: 36px;
  border: 3px solid #e5e7eb;
  border-top-color: #1e40af;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.profile-header-card {
  background: linear-gradient(135deg, #1e40af 0%, #2563eb 100%);
  border-radius: 16px;
  padding: 24px 32px;
  color: #ffffff;
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 28px;
  box-shadow: 0 4px 12px rgba(30, 64, 175, 0.15);
  gap: 20px;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 20px;
}

.avatar-wrapper {
  position: relative;
  width: 72px;
  height: 72px;
  border-radius: 50%;
  cursor: pointer;
  overflow: hidden;
  border: 2.5px solid rgba(255, 255, 255, 0.7);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  flex-shrink: 0;
}

.avatar-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.avatar-circle {
  width: 100%;
  height: 100%;
  background-color: rgba(255, 255, 255, 0.25);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
  font-weight: 700;
  color: #ffffff;
}

.avatar-hover-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.45);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.2s;
  color: #ffffff;
}

.avatar-wrapper:hover .avatar-hover-overlay {
  opacity: 1;
}

.header-name-row {
  display: flex;
  align-items: center;
  gap: 10px;
}

.profile-name {
  font-size: 24px;
  font-weight: 700;
  margin: 0;
  color: #ffffff;
}

.verify-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 22px;
  height: 22px;
  border-radius: 50%;
  cursor: default;
}

.verify-badge.verified {
  background-color: #10b981;
  color: #ffffff;
  box-shadow: 0 0 0 2px rgba(255, 255, 255, 0.3);
}

.verify-badge.unverified {
  background-color: rgba(255, 255, 255, 0.2);
  color: #e2e8f0;
  border: 1px solid rgba(255, 255, 255, 0.4);
}

.header-stats {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 8px;
}

.stat-pill {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  background-color: rgba(255, 255, 255, 0.15);
  padding: 8px 16px;
  border-radius: 20px;
  font-size: 13px;
  color: #f8fafc;
  backdrop-filter: blur(4px);
  width: 220px;
  box-sizing: border-box;
}

.admin-access-banner {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: #ffffff;
  border: 1px solid #bfdbfe;
  border-left: 5px solid #2563eb;
  border-radius: 12px;
  padding: 16px 24px;
  margin-bottom: 24px;
  box-shadow: 0 2px 6px rgba(37, 99, 235, 0.06);
  gap: 16px;
}

.admin-banner-content {
  display: flex;
  align-items: center;
  gap: 16px;
}

.admin-banner-badge {
  background-color: #eff6ff;
  color: #1d4ed8;
  font-size: 12px;
  font-weight: 700;
  padding: 4px 10px;
  border-radius: 6px;
  border: 1px solid #dbeafe;
  text-transform: uppercase;
}

.admin-banner-text {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.admin-banner-text strong {
  font-size: 15px;
  color: #1e293b;
}

.admin-banner-text span {
  font-size: 13px;
  color: #64748b;
}

.btn-admin-link {
  background-color: #2563eb;
  color: #ffffff;
  padding: 8px 18px;
  border-radius: 8px;
  font-size: 13px;
  font-weight: 600;
  text-decoration: none;
  white-space: nowrap;
  transition: all 0.15s ease;
}

.btn-admin-link:hover {
  background-color: #1d4ed8;
  transform: translateY(-1px);
}

.stat-icon {
  font-size: 14px;
}

@media (max-width: 768px) {
  .profile-header-card {
    flex-direction: column;
    align-items: flex-start;
    padding: 20px;
  }
  .header-stats {
    align-items: flex-start;
  }
}

.profile-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 24px;
  margin-bottom: 28px;
}

.profile-grid.single-col {
  grid-template-columns: 1fr;
}

@media (max-width: 800px) {
  .profile-grid {
    grid-template-columns: 1fr;
  }
}

.section-card {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
}

.section-header {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 20px;
  padding-bottom: 12px;
  border-bottom: 1px solid #f3f4f6;
}

.section-header.space-between {
  justify-content: space-between;
}

.header-left-title {
  display: flex;
  align-items: center;
  gap: 10px;
}

.section-header h3 {
  font-size: 17px;
  font-weight: 600;
  color: #111827;
  margin: 0;
}

.section-icon {
  width: 20px;
  height: 20px;
  color: #1e40af;
}

.books-count {
  font-size: 14px;
  color: #6b7280;
  font-weight: 500;
}

.profile-form {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.form-group label {
  font-size: 13px;
  font-weight: 600;
  color: #374151;
}

.form-input {
  width: 100%;
  padding: 10px 14px;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 14px;
  outline: none;
  transition: border-color 0.15s, box-shadow 0.15s;
}

.form-input:focus {
  border-color: #1e40af;
  box-shadow: 0 0 0 3px rgba(30, 64, 175, 0.1);
}

.form-input-disabled {
  background-color: #f9fafb;
  color: #6b7280;
  cursor: not-allowed;
  border-color: #e5e7eb;
}

.field-hint {
  font-size: 12px;
  color: #6b7280;
}

.form-actions {
  margin-top: 8px;
}

.btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  font-size: 14px;
  font-weight: 600;
  padding: 10px 20px;
  border-radius: 8px;
  cursor: pointer;
  border: none;
  transition: all 0.15s;
  text-decoration: none;
}

.btn-primary {
  background-color: #1e40af;
  color: #ffffff;
}

.btn-primary:hover:not(:disabled) {
  background-color: #1e3a8a;
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-outline-sm {
  font-size: 13px;
  font-weight: 600;
  padding: 6px 12px;
  border-radius: 6px;
  border: 1px solid #1e40af;
  color: #1e40af;
  background: transparent;
  text-decoration: none;
}

.btn-outline-sm:hover {
  background-color: #eff6ff;
}

.verification-status-banner {
  display: flex;
  align-items: flex-start;
  gap: 14px;
  padding: 16px;
  border-radius: 10px;
  margin-bottom: 16px;
}

.banner-pending {
  background-color: #fffbeb;
  border: 1px solid #fde68a;
  color: #92400e;
}

.banner-rejected {
  background-color: #fef2f2;
  border: 1px solid #fecaca;
  color: #991b1b;
}

.text-rejected {
  color: #dc2626 !important;
}

.verification-status-banner h4 {
  font-size: 15px;
  font-weight: 600;
  margin: 0 0 4px 0;
}

.verification-status-banner p {
  font-size: 13px;
  margin: 0;
  line-height: 1.4;
}

.verification-desc {
  font-size: 13px;
  color: #4b5563;
  margin-bottom: 16px;
  line-height: 1.5;
}

.upload-box.single-upload {
  margin-bottom: 16px;
}

.file-drop-area {
  border: 2px dashed #d1d5db;
  border-radius: 10px;
  height: 160px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  overflow: hidden;
  background-color: #f9fafb;
  transition: border-color 0.15s, background-color 0.15s;
}

.file-drop-area:hover {
  border-color: #1e40af;
  background-color: #f0fdf4;
}

.file-drop-area.has-file {
  border-style: solid;
  border-color: #10b981;
}

.hidden-file-input {
  display: none;
}

.upload-preview {
  width: 100%;
  height: 100%;
  object-fit: contain;
  background-color: #f3f4f6;
}

.upload-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 12px;
  text-align: center;
  gap: 6px;
}

.upload-title {
  font-size: 13px;
  font-weight: 600;
  color: #374151;
}

.upload-placeholder small {
  font-size: 11px;
  color: #9ca3af;
}

.sub-loading {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  padding: 40px 0;
  color: #6b7280;
  font-size: 14px;
}

.sub-loading .spinner {
  width: 24px;
  height: 24px;
}

.empty-books-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 48px 16px;
  text-align: center;
}

.empty-icon {
  width: 60px;
  height: 60px;
  margin-bottom: 12px;
}

.empty-title {
  font-size: 15px;
  font-weight: 600;
  color: #374151;
  margin-bottom: 4px;
}

.empty-hint {
  font-size: 13px;
  color: #6b7280;
  max-width: 420px;
  margin-bottom: 8px;
}

.mt-3 {
  margin-top: 12px;
}

.my-books-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 16px;
}

.book-card {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  overflow: hidden;
  text-decoration: none;
  color: inherit;
  display: flex;
  flex-direction: column;
  transition: transform 0.15s, box-shadow 0.15s;
}

.book-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.08);
}

.book-card-img {
  position: relative;
  width: 100%;
  height: 160px;
  background-color: #f3f4f6;
  overflow: hidden;
}

.book-card-img img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.book-card-noimg {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.book-card-badge {
  position: absolute;
  top: 8px;
  left: 8px;
  background-color: rgba(17, 24, 39, 0.75);
  color: #ffffff;
  font-size: 10px;
  font-weight: 600;
  padding: 2px 6px;
  border-radius: 4px;
}

.book-card-status {
  position: absolute;
  top: 8px;
  right: 8px;
  font-size: 10px;
  font-weight: 600;
  padding: 2px 6px;
  border-radius: 4px;
  color: #ffffff;
}

.book-card-status.sold {
  background-color: #ef4444;
}

.book-card-status.removed {
  background-color: #6b7280;
}

.book-card-body {
  padding: 12px;
  display: flex;
  flex-direction: column;
  flex: 1;
}

.book-card-title {
  font-size: 14px;
  font-weight: 600;
  color: #111827;
  margin: 0 0 6px 0;
  line-height: 1.3;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.book-card-price {
  font-size: 14px;
  font-weight: 700;
  color: #1e40af;
  margin: 0 0 8px 0;
}

.book-card-footer {
  margin-top: auto;
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 11px;
}

.status-indicator {
  font-weight: 600;
}

.status-0 {
  color: #10b981;
}

.status-1 {
  color: #ef4444;
}

.status-2 {
  color: #6b7280;
}

.date-text {
  color: #9ca3af;
}
</style>
