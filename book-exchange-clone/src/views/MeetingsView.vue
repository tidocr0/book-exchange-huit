<template>
  <div class="meetings-page">
    <div class="header">
      <h1>Quản lý lịch hẹn</h1>
      <p class="subtitle">Theo dõi các lịch hẹn mua bán sách của bạn.</p>
    </div>

    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Đang tải dữ liệu...</p>
    </div>

    <div v-else-if="meetings.length === 0" class="empty-state">
      <p>Bạn chưa có lịch hẹn nào.</p>
    </div>

    <div v-else class="meetings-list">
      <div v-for="m in meetings" :key="m.meetingId" class="meeting-card">
        <div class="card-header">
          <span class="role-badge" :class="m.isSeller ? 'role-seller' : 'role-buyer'">
            {{ m.isSeller ? 'Bạn là Người bán' : 'Bạn là Người mua' }}
          </span>
          <span class="status-badge" :class="getStatusClass(m.status)">
            {{ getStatusText(m.status) }}
          </span>
        </div>
        
        <div class="card-body">
          <h3 class="listing-title">
            <router-link :to="`/listing/${m.listingId}`">{{ m.listingTitle }}</router-link>
          </h3>
          <div class="info-row">
            <span class="label">Thời gian hẹn:</span>
            <span class="value font-semibold">{{ formatDateTime(m.proposedTime) }}</span>
          </div>
          <div class="info-row">
            <span class="label">Địa điểm:</span>
            <span class="value font-semibold text-location">📍 {{ m.location || 'Chưa xác định' }}</span>
          </div>
          <div class="info-row">
            <span class="label">Đối tác:</span>
            <span class="value">{{ m.partnerName }} - {{ m.partnerPhone || 'Chưa cung cấp SĐT' }}</span>
          </div>
          <div class="info-row">
            <span class="label">Ngày tạo yêu cầu:</span>
            <span class="value">{{ formatDate(m.createdAt) }}</span>
          </div>
        </div>

        <div class="card-actions">
          <template v-if="m.status === 0"> <!-- Pending -->
            <button v-if="m.isSeller" @click="updateStatus(m.meetingId, 1)" class="btn btn-primary btn-sm">Xác nhận</button>
            <button @click="updateStatus(m.meetingId, 2)" class="btn btn-danger btn-sm">Từ chối/Huỷ</button>
          </template>
          
          <template v-else-if="m.status === 1"> <!-- Accepted -->
            <button @click="updateStatus(m.meetingId, 3)" class="btn btn-success btn-sm">Đánh dấu Hoàn tất</button>
            <button @click="updateStatus(m.meetingId, 2)" class="btn btn-danger btn-sm">Huỷ lịch</button>
          </template>
          
          <template v-else-if="m.status === 3"> <!-- Completed -->
            <button class="btn btn-outline btn-sm" disabled>Đã hoàn tất</button>
            <!-- Nút đánh giá có thể thêm ở đây -->
          </template>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { getMyMeetings, updateMeetingStatus } from '../services/api';
import { useToast } from '../composables/useToast';

const { showToast } = useToast();
const meetings = ref([]);
const loading = ref(true);

const loadMeetings = async () => {
  try {
    const res = await getMyMeetings();
    meetings.value = res.data;
  } catch (error) {
    console.error(error);
  } finally {
    loading.value = false;
  }
};

const updateStatus = async (id, status) => {
  if (!confirm('Bạn có chắc chắn muốn cập nhật trạng thái lịch hẹn này?')) return;
  try {
    await updateMeetingStatus(id, status);
    showToast('Cập nhật thành công!', 'success');
    await loadMeetings();
  } catch (error) {
    showToast(error.response?.data?.message || 'Có lỗi xảy ra', 'error');
  }
};

const getStatusText = (status) => {
  switch (status) {
    case 0: return 'Chờ xác nhận';
    case 1: return 'Đã xác nhận';
    case 2: return 'Đã huỷ';
    case 3: return 'Hoàn tất';
    default: return 'Không xác định';
  }
};

const getStatusClass = (status) => {
  switch (status) {
    case 0: return 'status-pending';
    case 1: return 'status-accepted';
    case 2: return 'status-rejected';
    case 3: return 'status-completed';
    default: return '';
  }
};

const formatDateTime = (dateStr) => {
  const d = new Date(dateStr);
  return d.toLocaleDateString('vi-VN') + ' - ' + d.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
};

const formatDate = (dateStr) => {
  const d = new Date(dateStr);
  return d.toLocaleDateString('vi-VN');
};

onMounted(() => {
  loadMeetings();
});
</script>

<style scoped>
.meetings-page {
  max-width: 800px;
  margin: 0 auto;
  padding: 32px 0;
}

.header {
  margin-bottom: 24px;
}

.header h1 {
  font-size: 28px;
  font-weight: 700;
  color: #111827;
  margin: 0 0 8px 0;
}

.subtitle {
  color: #6b7280;
  margin: 0;
}

.meetings-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.meeting-card {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
  padding-bottom: 12px;
  border-bottom: 1px solid #f3f4f6;
}

.role-badge {
  font-size: 13px;
  font-weight: 600;
  padding: 4px 10px;
  border-radius: 16px;
}

.role-seller {
  background-color: #dbeafe;
  color: #1e40af;
}

.role-buyer {
  background-color: #f3f4f6;
  color: #374151;
}

.status-badge {
  font-size: 13px;
  font-weight: 600;
  padding: 4px 10px;
  border-radius: 6px;
}

.status-pending { background-color: #fef08a; color: #854d0e; }
.status-accepted { background-color: #dcfce3; color: #166534; }
.status-rejected { background-color: #fee2e2; color: #991b1b; }
.status-completed { background-color: #e0e7ff; color: #3730a3; }

.card-body {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-bottom: 20px;
}

.listing-title {
  font-size: 18px;
  font-weight: 600;
  margin: 0 0 8px 0;
}

.listing-title a {
  color: #111827;
  text-decoration: none;
}

.listing-title a:hover {
  color: #1e40af;
}

.info-row {
  display: flex;
  font-size: 15px;
}

.label {
  width: 140px;
  color: #6b7280;
}

.value {
  color: #111827;
}

.font-semibold {
  font-weight: 600;
  color: #1e40af;
}

.text-location {
  color: #047857;
}

.card-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
}

.btn {
  font-family: inherit;
  font-size: 14px;
  font-weight: 500;
  padding: 8px 16px;
  border-radius: 6px;
  cursor: pointer;
  border: none;
  transition: all 0.15s;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-primary { background-color: #1e40af; color: #fff; }
.btn-primary:hover:not(:disabled) { background-color: #1e3a8a; }

.btn-success { background-color: #10b981; color: #fff; }
.btn-success:hover:not(:disabled) { background-color: #059669; }

.btn-danger { background-color: #ef4444; color: #fff; }
.btn-danger:hover:not(:disabled) { background-color: #b91c1c; }

.btn-outline { background: transparent; border: 1px solid #d1d5db; color: #374151; }
.btn-outline:hover:not(:disabled) { background-color: #f3f4f6; }

.empty-state {
  text-align: center;
  padding: 40px;
  color: #6b7280;
  font-size: 16px;
  background: #ffffff;
  border-radius: 12px;
  border: 1px solid #e5e7eb;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 40px;
}

.spinner {
  width: 32px;
  height: 32px;
  border: 3px solid #f3f4f6;
  border-top-color: #1e40af;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 12px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
