<template>
  <div class="detail-page" v-if="listing">
    <div class="detail-container">
      <div class="detail-left">
        <div class="main-image-wrapper">
          <img
            v-if="listing.images && listing.images.length > 0"
            :src="listing.images[0].imageUrl.startsWith('http') ? listing.images[0].imageUrl : 'http://localhost:5100' + listing.images[0].imageUrl"
            alt="Book Image"
            class="main-image"
            @error="e => e.target.src = 'https://via.placeholder.com/400x500?text=No+Image'"
          />
          <div v-else class="no-image">
            <svg width="48" height="48" viewBox="0 0 24 24" fill="none"><path d="M4 19.5A2.5 2.5 0 016.5 17H20" stroke="#9ca3af" stroke-width="1.5" stroke-linecap="round"/><path d="M6.5 2H20v20H6.5A2.5 2.5 0 014 19.5v-15A2.5 2.5 0 016.5 2z" stroke="#9ca3af" stroke-width="1.5" stroke-linecap="round"/></svg>
            <p>Chưa có hình ảnh</p>
          </div>
        </div>
        <div class="thumbnails" v-if="listing.images && listing.images.length > 1">
          <img
            v-for="(img, index) in listing.images.slice(1)"
            :key="img.imageId"
            :src="img.imageUrl.startsWith('http') ? img.imageUrl : 'http://localhost:5100' + img.imageUrl"
            alt="Thumbnail"
            class="thumbnail"
            @error="e => e.target.style.display = 'none'"
          />
        </div>
      </div>

      <div class="detail-right">
        <div class="detail-header">
          <div class="badge-row">
            <span class="badge condition-badge">{{ getConditionText(listing.condition) }}</span>
            <span v-if="listing.status === 1" class="badge status-sold">Đã bán</span>
            <span v-if="listing.status === 2" class="badge status-removed">Đã gỡ</span>
          </div>
          <h1 class="title">{{ listing.title }}</h1>
          <p class="price">{{ listing.price.toLocaleString('vi-VN') }} đ</p>
        </div>

        <div class="info-section">
          <h3 class="section-title">Mô tả chi tiết</h3>
          <p class="description">{{ listing.description }}</p>
        </div>

        <div class="seller-card" v-if="listing.seller">
          <h3 class="section-title">Thông tin người bán</h3>
          <div class="seller-details">
            <div class="seller-row">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none"><circle cx="12" cy="8" r="4" stroke="#6b7280" stroke-width="2"/><path d="M4 20c0-4 4-7 8-7s8 3 8 7" stroke="#6b7280" stroke-width="2" stroke-linecap="round"/></svg>
              <span>{{ listing.seller.fullName }}</span>
              <span v-if="listing.seller.isVerified" class="verified-icon" title="Đã xác thực">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="#10b981"><path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z"/></svg>
              </span>
            </div>
            <div class="seller-row">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none"><path d="M22 16.92v3a2 2 0 01-2.18 2 19.79 19.79 0 01-8.63-3.07 19.5 19.5 0 01-6-6 19.79 19.79 0 01-3.07-8.67A2 2 0 014.11 2h3a2 2 0 012 1.72 12.84 12.84 0 00.7 2.81 2 2 0 01-.45 2.11L8.09 9.91a16 16 0 006 6l1.27-1.27a2 2 0 012.11-.45 12.84 12.84 0 002.81.7A2 2 0 0122 16.92z" stroke="#6b7280" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>
              <span>{{ listing.seller.phone || 'Chưa cung cấp' }}</span>
            </div>
          </div>
        </div>

        <div class="action-section">
          <div v-if="isOwner" class="owner-actions">
            <button @click="changeStatus(1)" class="btn btn-outline" :disabled="listing.status !== 0">Đánh dấu Đã Bán</button>
            <button @click="changeStatus(2)" class="btn btn-danger" :disabled="listing.status !== 0">Gỡ tin</button>
          </div>
          <div v-else class="buyer-actions">
            <div v-if="!showBooking">
              <button @click="openBooking" class="btn btn-primary btn-block">Đặt lịch hẹn</button>
            </div>
            <div v-else class="booking-section">
              <h4 class="booking-title">Chọn thời gian giao dịch</h4>
              
              <div v-if="loadingDates" class="loading-inline">Đang tải lịch rảnh của người bán...</div>
              
              <div v-else-if="availableDates.length === 0" class="no-dates">
                Người bán chưa thiết lập lịch rảnh hoặc đã kín lịch trong 14 ngày tới.
              </div>
              
              <div v-else class="booking-controls">
                <div class="dates-scroll">
                  <div 
                    v-for="d in availableDates" 
                    :key="d.date"
                    class="date-card"
                    :class="{ active: selectedDate === d.date }"
                    @click="selectDate(d)"
                  >
                    <span class="d-dow">{{ getDayOfWeekName(d.date) }}</span>
                    <span class="d-date">{{ getDayMonth(d.date) }}</span>
                  </div>
                </div>
                
                <div v-if="selectedDate" class="slots-container">
                  <button 
                    v-for="slot in currentSlots" 
                    :key="slot"
                    class="slot-btn"
                    :class="{ active: selectedSlot === slot }"
                    @click="selectedSlot = slot"
                  >
                    {{ getSlotName(slot) }}
                  </button>
                </div>

                <div v-if="selectedDate" class="booking-field-group">
                  <label class="booking-label">Địa điểm gặp mặt:</label>
                  <div class="location-chips">
                    <button 
                      type="button"
                      v-for="loc in presetLocations" 
                      :key="loc"
                      class="chip-btn"
                      :class="{ active: selectedLocationType === loc }"
                      @click="selectedLocationType = loc"
                    >
                      {{ loc }}
                    </button>
                    <button 
                      type="button"
                      class="chip-btn"
                      :class="{ active: selectedLocationType === 'other' }"
                      @click="selectedLocationType = 'other'"
                    >
                      Khác...
                    </button>
                  </div>
                  <input 
                    v-if="selectedLocationType === 'other'"
                    type="text"
                    v-model="customLocation"
                    placeholder="Nhập địa điểm hẹn (VD: Quán cafe, KTX...)"
                    class="custom-location-input"
                  />
                </div>
                
                <button 
                  @click="submitBooking" 
                  class="btn btn-primary btn-block mt-3"
                  :disabled="selectedSlot === null || submittingBooking"
                >
                  {{ submittingBooking ? 'Đang gửi yêu cầu...' : 'Gửi yêu cầu đặt lịch' }}
                </button>
                <button @click="showBooking = false" class="btn btn-outline btn-block mt-2">Hủy</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
  
  <div v-else-if="loading" class="state-container">
    <div class="spinner"></div>
    <p class="state-title">Đang tải chi tiết...</p>
  </div>
  
  <div v-else class="state-container">
    <svg class="state-icon" viewBox="0 0 80 80" fill="none" xmlns="http://www.w3.org/2000/svg">
      <circle cx="40" cy="40" r="40" fill="#F3F4F6"/>
      <path d="M40 25C31.7 25 25 31.7 25 40C25 48.3 31.7 55 40 55C48.3 55 55 48.3 55 40C55 31.7 48.3 25 40 25ZM40 51C33.9 51 29 46.1 29 40C29 33.9 33.9 29 40 29C46.1 29 51 33.9 51 40C51 46.1 46.1 51 40 51Z" fill="#9CA3AF"/>
      <path d="M41 33H39V41H41V33Z" fill="#9CA3AF"/>
      <path d="M41 43H39V45H41V43Z" fill="#9CA3AF"/>
    </svg>
    <h3 class="state-title">Không tìm thấy tin đăng</h3>
    <p class="state-desc">Tin đăng này có thể đã bị xóa hoặc không tồn tại.</p>
    <router-link to="/" class="btn btn-primary" style="margin-top: 16px; display: inline-block;">Quay lại Khám phá</router-link>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { getListingDetail, updateListingStatus, getAvailableDates, createMeeting } from '../services/api';
import { useToast } from '../composables/useToast';

const { showToast } = useToast();
const route = useRoute();
const router = useRouter();
const listing = ref(null);
const loading = ref(true);

const user = ref(JSON.parse(localStorage.getItem('user') || '{}'));
const isOwner = computed(() => listing.value && user.value.userId === listing.value.sellerId);

const getConditionText = (condition) => {
  switch (condition) {
    case 0: return 'Sách gốc - Mới';
    case 1: return 'Sách gốc - Đã qua sử dụng';
    case 2: return 'Sách photo - Mới';
    case 3: return 'Sách photo - Đã qua sử dụng';
    default: return '';
  }
};

const fetchDetail = async () => {
  try {
    const res = await getListingDetail(route.params.id);
    listing.value = res.data;
  } catch (error) {
    console.error(error);
  } finally {
    loading.value = false;
  }
};

const changeStatus = async (status) => {
  if (!confirm('Bạn có chắc muốn chuyển trạng thái?')) return;
  try {
    await updateListingStatus(listing.value.listingId, status);
    showToast('Đổi trạng thái thành công!', 'success');
    await fetchDetail();
  } catch (err) {
    showToast(err.response?.data?.message || 'Có lỗi xảy ra', 'error');
  }
};

// Booking Logic
const showBooking = ref(false);
const loadingDates = ref(false);
const availableDates = ref([]);
const selectedDate = ref(null);
const currentSlots = ref([]);
const selectedSlot = ref(null);
const submittingBooking = ref(false);

const openBooking = async () => {
  if (!user.value || !user.value.userId) {
    showToast('Vui lòng đăng nhập để đặt lịch hẹn.', 'error');
    router.push('/login');
    return;
  }
  showBooking.value = true;
  loadingDates.value = true;
  try {
    const res = await getAvailableDates(listing.value.sellerId);
    availableDates.value = res.data;
  } catch (err) {
    console.error(err);
    showToast('Không thể tải lịch rảnh của người bán.', 'error');
  } finally {
    loadingDates.value = false;
  }
};

const selectDate = (d) => {
  selectedDate.value = d.date;
  currentSlots.value = d.availableSlots;
  selectedSlot.value = null;
};

const getDayOfWeekName = (dateStr) => {
  const d = new Date(dateStr);
  const days = ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'];
  return days[d.getDay()];
};

const getDayMonth = (dateStr) => {
  const d = new Date(dateStr);
  return `${d.getDate()}/${d.getMonth() + 1}`;
};

const presetLocations = [
  'Thư viện trường',
  'Căn tin trường',
  'Sảnh toà A',
  'Cổng trường (140 Lê Trọng Tấn)'
];
const selectedLocationType = ref('Thư viện trường');
const customLocation = ref('');

const finalLocation = computed(() => {
  if (selectedLocationType.value === 'other') {
    return customLocation.value.trim();
  }
  return selectedLocationType.value;
});

const getSlotName = (slot) => {
  switch(slot) {
    case 0: return '07:00 - 09:00';
    case 1: return '09:00 - 11:00';
    case 2: return '11:00 - 13:00';
    case 3: return '13:00 - 15:00';
    case 4: return '15:00 - 17:00';
    case 5: return '17:00 - 19:00';
    case 6: return '19:00 - 21:00';
    default: return '';
  }
};

const getHourFromSlot = (slot) => {
  switch(slot) {
    case 0: return 8;
    case 1: return 10;
    case 2: return 12;
    case 3: return 14;
    case 4: return 16;
    case 5: return 18;
    case 6: return 20;
    default: return 8;
  }
};

const submitBooking = async () => {
  if (selectedSlot.value === null || !selectedDate.value) return;
  if (!finalLocation.value) {
    showToast('Vui lòng chọn hoặc nhập địa điểm gặp mặt.', 'error');
    return;
  }
  submittingBooking.value = true;
  try {
    const d = new Date(selectedDate.value);
    d.setHours(getHourFromSlot(selectedSlot.value), 0, 0, 0);

    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    const hour = String(d.getHours()).padStart(2, '0');
    const proposedTime = `${year}-${month}-${day}T${hour}:00:00`;

    await createMeeting({
      listingId: listing.value.listingId,
      proposedTime: proposedTime,
      location: finalLocation.value
    });
    showToast('Đã gửi yêu cầu đặt lịch hẹn thành công!', 'success');
    router.push('/meetings');
  } catch (err) {
    showToast(err.response?.data?.message || 'Có lỗi khi đặt lịch hẹn.', 'error');
  } finally {
    submittingBooking.value = false;
  }
};

onMounted(fetchDetail);
</script>

<style scoped>
.detail-page {
  padding: 32px 0;
  max-width: 1000px;
  margin: 0 auto;
}

.detail-container {
  display: flex;
  gap: 40px;
  background: #ffffff;
  border-radius: 16px;
  border: 1px solid #e5e7eb;
  padding: 32px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05);
}

.detail-left {
  flex: 0 0 400px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.main-image-wrapper {
  width: 100%;
  aspect-ratio: 4/5;
  border-radius: 12px;
  overflow: hidden;
  background-color: #f3f4f6;
  border: 1px solid #e5e7eb;
  display: flex;
  align-items: center;
  justify-content: center;
}

.main-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.no-image {
  display: flex;
  flex-direction: column;
  align-items: center;
  color: #9ca3af;
  gap: 8px;
}

.thumbnails {
  display: flex;
  gap: 12px;
  overflow-x: auto;
  padding-bottom: 8px;
}

.thumbnail {
  width: 80px;
  height: 80px;
  border-radius: 8px;
  object-fit: cover;
  border: 1px solid #e5e7eb;
  cursor: pointer;
}

.detail-right {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.badge-row {
  display: flex;
  gap: 8px;
  margin-bottom: 12px;
}

.badge {
  font-size: 13px;
  font-weight: 600;
  padding: 4px 10px;
  border-radius: 6px;
}

.condition-badge {
  background-color: #f3f4f6;
  color: #4b5563;
}

.status-sold {
  background-color: #fee2e2;
  color: #dc2626;
}

.status-removed {
  background-color: #f3f4f6;
  color: #6b7280;
}

.title {
  font-size: 28px;
  font-weight: 700;
  color: #111827;
  line-height: 1.3;
  margin: 0 0 12px 0;
}

.price {
  font-size: 24px;
  font-weight: 700;
  color: #dc2626;
  margin: 0;
}

.section-title {
  font-size: 16px;
  font-weight: 600;
  color: #111827;
  margin: 0 0 12px 0;
}

.description {
  font-size: 15px;
  line-height: 1.6;
  color: #4b5563;
  white-space: pre-wrap;
  margin: 0;
}

.seller-card {
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  padding: 20px;
}

.seller-details {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.seller-row {
  display: flex;
  align-items: center;
  gap: 10px;
  color: #374151;
  font-size: 15px;
  font-weight: 500;
}

.verified-icon {
  display: flex;
  align-items: center;
}

.action-section {
  margin-top: auto;
  padding-top: 24px;
  border-top: 1px solid #e5e7eb;
}

.owner-actions {
  display: flex;
  gap: 12px;
}

.btn {
  font-size: 15px;
  font-weight: 600;
  padding: 12px 20px;
  border-radius: 8px;
  text-align: center;
  cursor: pointer;
  border: none;
  font-family: inherit;
  transition: all 0.15s;
  text-decoration: none;
}

.btn-primary {
  background-color: #1e40af;
  color: #ffffff;
}

.btn-primary:hover {
  background-color: #1e3a8a;
}

.btn-outline {
  background: transparent;
  color: #374151;
  border: 1.5px solid #d1d5db;
}

.btn-outline:hover:not(:disabled) {
  background-color: #f3f4f6;
}

.btn-danger {
  background-color: #fee2e2;
  color: #dc2626;
}

.btn-danger:hover:not(:disabled) {
  background-color: #fecaca;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-block {
  display: block;
  width: 100%;
}

.state-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 80px 20px;
  text-align: center;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 3px solid #f3f4f6;
  border-top-color: #1e40af;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 20px;
}

.state-icon {
  width: 80px;
  height: 80px;
  margin-bottom: 24px;
}

.state-title {
  font-size: 20px;
  font-weight: 600;
  color: #111827;
  margin: 0 0 8px 0;
}

.state-desc {
  font-size: 15px;
  color: #6b7280;
  margin: 0;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* Booking UI */
.booking-section {
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  padding: 20px;
}

.booking-title {
  font-size: 16px;
  font-weight: 600;
  margin: 0 0 16px 0;
  color: #111827;
}

.dates-scroll {
  display: flex;
  gap: 12px;
  overflow-x: auto;
  padding-bottom: 8px;
  margin-bottom: 16px;
}

.date-card {
  flex: 0 0 auto;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 60px;
  height: 64px;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  background: #fff;
  cursor: pointer;
  transition: all 0.15s;
}

.date-card:hover {
  border-color: #9ca3af;
}

.date-card.active {
  background: #1e40af;
  border-color: #1e40af;
  color: #fff;
}

.date-card.active .d-dow, .date-card.active .d-date {
  color: #fff;
}

.d-dow {
  font-size: 12px;
  color: #6b7280;
  font-weight: 500;
}

.d-date {
  font-size: 14px;
  color: #111827;
  font-weight: 600;
}

.slots-container {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
  margin-bottom: 16px;
}

.slot-btn {
  padding: 10px;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  background: #fff;
  color: #374151;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.15s;
}

.slot-btn:hover {
  border-color: #9ca3af;
}

.slot-btn.active {
  background: #eff6ff;
  border-color: #1e40af;
  color: #1e40af;
}

.booking-field-group {
  margin-top: 14px;
  margin-bottom: 14px;
}

.booking-label {
  display: block;
  font-size: 13px;
  font-weight: 600;
  color: #374151;
  margin-bottom: 8px;
}

.location-chips {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
}

.chip-btn {
  padding: 6px 12px;
  font-size: 12px;
  border: 1px solid #d1d5db;
  border-radius: 16px;
  background: #fff;
  color: #4b5563;
  cursor: pointer;
  transition: all 0.15s;
}

.chip-btn:hover {
  border-color: #9ca3af;
  background: #f9fafb;
}

.chip-btn.active {
  background: #eff6ff;
  border-color: #1e40af;
  color: #1e40af;
  font-weight: 600;
}

.custom-location-input {
  width: 100%;
  margin-top: 8px;
  padding: 8px 12px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 13px;
  outline: none;
}

.custom-location-input:focus {
  border-color: #1e40af;
}

.mt-2 { margin-top: 8px; }
.mt-3 { margin-top: 16px; }

.loading-inline, .no-dates {
  font-size: 14px;
  color: #6b7280;
  text-align: center;
  padding: 20px 0;
}

@media (max-width: 768px) {
  .detail-container {
    flex-direction: column;
    padding: 20px;
  }
  
  .detail-left {
    flex: none;
    width: 100%;
  }
}
</style>
