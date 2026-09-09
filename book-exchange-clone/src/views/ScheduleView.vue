<template>
  <div class="schedule-page">
    <div class="header">
      <h1>Lịch của tôi</h1>
      <p class="subtitle">Thiết lập thời gian rảnh để người mua có thể đặt lịch hẹn giao dịch với bạn.</p>
    </div>

    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Đang tải dữ liệu...</p>
    </div>
    
    <div v-else class="content-wrapper">
      <section class="card">
        <div class="card-header">
          <h2>Lịch rảnh cố định hàng tuần</h2>
          <div class="card-actions">
            <button @click="selectAll" class="btn btn-sm btn-outline">Chọn tất cả</button>
            <button @click="clearAll" class="btn btn-sm btn-outline">Xoá tất cả</button>
            <button @click="saveSchedule" class="btn btn-sm btn-primary" :disabled="saving">
              {{ saving ? 'Đang lưu...' : 'Lưu lịch' }}
            </button>
          </div>
        </div>
        <p class="section-desc">Chọn các khung giờ bạn có thể gặp mặt giao dịch mỗi tuần.</p>
        
        <div class="schedule-grid-container">
          <div class="schedule-grid">
            <div class="grid-header"></div>
            <div class="grid-header" v-for="slot in timeSlots" :key="slot.value">{{ slot.label }}</div>
            
            <template v-for="day in daysOfWeek" :key="day.value">
              <div class="grid-row-label">{{ day.label }}</div>
              <div 
                v-for="slot in timeSlots" 
                :key="`${day.value}-${slot.value}`"
                class="grid-cell"
                :class="{ selected: isSelected(day.value, slot.value) }"
                @click="toggleSlot(day.value, slot.value)"
              >
                <svg v-if="isSelected(day.value, slot.value)" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
                  <polyline points="20 6 9 17 4 12"></polyline>
                </svg>
              </div>
            </template>
          </div>
        </div>
      </section>

      <section class="card">
        <div class="card-header">
          <h2>Ngày bận đột xuất</h2>
        </div>
        <p class="section-desc">Thêm các ngày bạn bận không thể giao dịch (sẽ ghi đè lên lịch rảnh cố định).</p>
        
        <form @submit.prevent="addBlackout" class="add-blackout-form">
          <div class="form-group">
            <label>Ngày bận</label>
            <input type="date" v-model="newBlackout.date" :min="minDate" required class="input-field" />
          </div>
          <div class="form-group">
            <label>Khung giờ</label>
            <select v-model="newBlackout.timeSlot" class="input-field">
              <option :value="null">Bận cả ngày</option>
              <option v-for="slot in timeSlots" :key="slot.value" :value="slot.value">{{ slot.label }}</option>
            </select>
          </div>
          <button type="submit" class="btn btn-primary" :disabled="addingBlackout">Thêm</button>
        </form>

        <div class="blackout-list" v-if="blackoutDates.length > 0">
          <div v-for="b in blackoutDates" :key="b.blackoutId" class="blackout-tag">
            <div class="blackout-info">
              <span class="blackout-date">{{ formatDate(b.blackoutDate) }}</span>
              <span class="blackout-slot">{{ formatSlot(b.timeSlot) }}</span>
            </div>
            <button @click="removeBlackout(b.blackoutId)" class="remove-btn" title="Xoá">
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <line x1="18" y1="6" x2="6" y2="18"></line>
                <line x1="6" y1="6" x2="18" y2="18"></line>
              </svg>
            </button>
          </div>
        </div>
        <p v-else class="empty-state">Chưa có ngày bận đột xuất nào.</p>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { getMySchedule, updateMySchedule, addBlackoutDate, removeBlackoutDate } from '../services/api';
import { useToast } from '../composables/useToast';

const { showToast } = useToast();
const loading = ref(true);
const saving = ref(false);
const addingBlackout = ref(false);

const selectedSlots = ref([]);
const blackoutDates = ref([]);

const today = new Date();
const minDate = today.toISOString().split('T')[0];

const newBlackout = ref({
  date: '',
  timeSlot: null
});

const daysOfWeek = [
  { value: 1, label: 'Thứ 2' },
  { value: 2, label: 'Thứ 3' },
  { value: 3, label: 'Thứ 4' },
  { value: 4, label: 'Thứ 5' },
  { value: 5, label: 'Thứ 6' },
  { value: 6, label: 'Thứ 7' },
  { value: 0, label: 'Chủ Nhật' }
];

const timeSlots = [
  { value: 0, label: '07:00 - 09:00' },
  { value: 1, label: '09:00 - 11:00' },
  { value: 2, label: '11:00 - 13:00' },
  { value: 3, label: '13:00 - 15:00' },
  { value: 4, label: '15:00 - 17:00' },
  { value: 5, label: '17:00 - 19:00' },
  { value: 6, label: '19:00 - 21:00' }
];

const loadSchedule = async () => {
  try {
    const res = await getMySchedule();
    selectedSlots.value = res.data.availabilities;
    blackoutDates.value = res.data.blackoutDates;
  } catch (error) {
    console.error(error);
    showToast('Không thể tải lịch của bạn.', 'error');
  } finally {
    loading.value = false;
  }
};

const isSelected = (day, slot) => {
  return selectedSlots.value.some(s => s.dayOfWeek === day && s.timeSlot === slot);
};

const toggleSlot = (day, slot) => {
  const index = selectedSlots.value.findIndex(s => s.dayOfWeek === day && s.timeSlot === slot);
  if (index >= 0) {
    selectedSlots.value.splice(index, 1);
  } else {
    selectedSlots.value.push({ dayOfWeek: day, timeSlot: slot });
  }
};

const selectAll = () => {
  const all = [];
  for (const d of daysOfWeek) {
    for (const s of timeSlots) {
      all.push({ dayOfWeek: d.value, timeSlot: s.value });
    }
  }
  selectedSlots.value = all;
};

const clearAll = () => {
  selectedSlots.value = [];
};

const saveSchedule = async () => {
  saving.value = true;
  try {
    await updateMySchedule(selectedSlots.value);
    showToast('Đã lưu lịch rảnh thành công!', 'success');
  } catch (err) {
    showToast(err.response?.data?.message || 'Có lỗi khi lưu lịch.', 'error');
  } finally {
    saving.value = false;
  }
};

const addBlackout = async () => {
  if (!newBlackout.value.date) return;
  addingBlackout.value = true;
  try {
    const res = await addBlackoutDate({
      blackoutDate: newBlackout.value.date,
      timeSlot: newBlackout.value.timeSlot
    });
    blackoutDates.value.push(res.data);
    newBlackout.value = { date: '', timeSlot: null };
    showToast('Đã thêm ngày bận.', 'success');
  } catch (err) {
    showToast(err.response?.data?.message || 'Có lỗi khi thêm ngày bận.', 'error');
  } finally {
    addingBlackout.value = false;
  }
};

const removeBlackout = async (id) => {
  try {
    await removeBlackoutDate(id);
    blackoutDates.value = blackoutDates.value.filter(b => b.blackoutId !== id);
    showToast('Đã xoá ngày bận.', 'success');
  } catch (err) {
    showToast('Không thể xoá ngày bận.', 'error');
  }
};

const formatDate = (dateStr) => {
  const d = new Date(dateStr);
  return d.toLocaleDateString('vi-VN');
};

const formatSlot = (slot) => {
  if (slot === null || slot === undefined) return 'Cả ngày';
  return timeSlots.find(s => s.value === slot)?.label || '';
};

onMounted(() => {
  loadSchedule();
});
</script>

<style scoped>
.schedule-page {
  max-width: 1000px;
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

.content-wrapper {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.card {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.card-header h2 {
  font-size: 20px;
  font-weight: 600;
  color: #111827;
  margin: 0;
}

.section-desc {
  color: #6b7280;
  margin: 0 0 20px 0;
  font-size: 15px;
}

.card-actions {
  display: flex;
  gap: 8px;
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

.btn-primary {
  background-color: #1e40af;
  color: #fff;
}

.btn-primary:hover:not(:disabled) {
  background-color: #1e3a8a;
}

.btn-outline {
  background: transparent;
  border: 1px solid #d1d5db;
  color: #374151;
}

.btn-outline:hover:not(:disabled) {
  background-color: #f3f4f6;
}

/* Grid layout */
.schedule-grid-container {
  overflow-x: auto;
  padding-bottom: 8px;
}

.schedule-grid {
  display: grid;
  grid-template-columns: 80px repeat(7, minmax(110px, 1fr));
  gap: 8px;
  margin-top: 16px;
  min-width: 860px;
}

.grid-header {
  font-weight: 600;
  color: #4b5563;
  text-align: center;
  padding: 12px 0;
  font-size: 14px;
}

.grid-row-label {
  display: flex;
  align-items: center;
  font-weight: 600;
  color: #374151;
  font-size: 15px;
}

.grid-cell {
  background: #f9fafb;
  border: 2px solid #e5e7eb;
  border-radius: 8px;
  height: 56px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.15s;
  color: #1e40af;
}

.grid-cell:hover {
  border-color: #d1d5db;
  background: #f3f4f6;
}

.grid-cell.selected {
  background: #eff6ff;
  border-color: #1e40af;
}

/* Blackout form */
.add-blackout-form {
  display: flex;
  align-items: flex-end;
  gap: 16px;
  margin-bottom: 24px;
  padding: 16px;
  background: #f9fafb;
  border-radius: 8px;
  border: 1px solid #e5e7eb;
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

.input-field {
  padding: 8px 12px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-family: inherit;
  outline: none;
}

.input-field:focus {
  border-color: #1e40af;
}

.blackout-list {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
}

.blackout-tag {
  display: flex;
  align-items: center;
  background: #fee2e2;
  border: 1px solid #fecaca;
  padding: 8px 12px;
  border-radius: 8px;
  gap: 12px;
}

.blackout-info {
  display: flex;
  flex-direction: column;
}

.blackout-date {
  font-weight: 600;
  color: #991b1b;
  font-size: 14px;
}

.blackout-slot {
  font-size: 12px;
  color: #b91c1c;
}

.remove-btn {
  background: transparent;
  border: none;
  color: #dc2626;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.remove-btn:hover {
  background: #fca5a5;
}

.empty-state {
  color: #9ca3af;
  font-style: italic;
  font-size: 14px;
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
