<template>
  <div class="listings-page">
    <div class="search-wrapper">
      <div class="search-bar">
        <svg class="search-icon" width="20" height="20" viewBox="0 0 20 20" fill="none">
          <circle cx="9" cy="9" r="6" stroke="#9ca3af" stroke-width="2"/>
          <path d="M13.5 13.5L17 17" stroke="#9ca3af" stroke-width="2" stroke-linecap="round"/>
        </svg>
        <input
          type="text"
          v-model="searchText"
          placeholder="Tìm kiếm tên sách, tác giả, môn học..."
          class="search-input"
          @keyup.enter="applySearch"
        />
        <button @click="applySearch" class="search-btn">Tìm kiếm</button>
      </div>
    </div>

    <div class="filter-card">
      <div class="filter-header">
        <span class="filter-label">Bộ lọc nâng cao</span>
        <button @click="clearFilters" class="reset-btn">
          <svg width="14" height="14" viewBox="0 0 14 14" fill="none">
            <path d="M1 7a6 6 0 1011.2-3" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/>
            <path d="M10 1v3h3" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
          Đặt lại
        </button>
      </div>
      <div class="filter-grid">
        <div class="filter-group">
          <label for="f-faculty">Khoa</label>
          <div class="select-wrapper">
            <select id="f-faculty" v-model="filters.facultyId" @change="onFacultyChange">
              <option value="">Tất cả Khoa</option>
              <option v-for="f in faculties" :key="f.facultyId" :value="f.facultyId">{{ f.name }}</option>
            </select>
            <svg class="select-arrow" width="12" height="12" viewBox="0 0 12 12"><path d="M3 4.5l3 3 3-3" stroke="#6b7280" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/></svg>
          </div>
        </div>
        <div class="filter-group">
          <label for="f-subject">Môn học</label>
          <div class="select-wrapper">
            <select id="f-subject" v-model="filters.subjectId" :disabled="!filters.facultyId">
              <option value="">Tất cả Môn</option>
              <option v-for="s in subjects" :key="s.subjectId" :value="s.subjectId">{{ s.name }}</option>
            </select>
            <svg class="select-arrow" width="12" height="12" viewBox="0 0 12 12"><path d="M3 4.5l3 3 3-3" stroke="#6b7280" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/></svg>
          </div>
        </div>
        <div class="filter-group">
          <label for="f-condition">Loại sách</label>
          <div class="select-wrapper">
            <select id="f-condition" v-model="filters.condition">
              <option value="">Tất cả</option>
              <option value="0">Sách gốc - Mới</option>
              <option value="1">Sách gốc - Đã qua sử dụng</option>
              <option value="2">Sách photo - Mới</option>
              <option value="3">Sách photo - Đã qua sử dụng</option>
            </select>
            <svg class="select-arrow" width="12" height="12" viewBox="0 0 12 12"><path d="M3 4.5l3 3 3-3" stroke="#6b7280" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/></svg>
          </div>
        </div>
        <div class="filter-group">
          <label for="f-price">Khoảng giá</label>
          <div class="select-wrapper">
            <select id="f-price" v-model="priceRange">
              <option value="">Tất cả mức giá</option>
              <option value="0-20000">Dưới 20.000đ</option>
              <option value="20000-50000">20.000đ - 50.000đ</option>
              <option value="50000-100000">50.000đ - 100.000đ</option>
              <option value="100000-200000">100.000đ - 200.000đ</option>
              <option value="200000-">Trên 200.000đ</option>
            </select>
            <svg class="select-arrow" width="12" height="12" viewBox="0 0 12 12"><path d="M3 4.5l3 3 3-3" stroke="#6b7280" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/></svg>
          </div>
        </div>
      </div>
    </div>

    <div v-if="loading" class="state-container">
      <div class="spinner"></div>
      <p class="state-title">Đang tải dữ liệu...</p>
    </div>

    <div v-else-if="listings.length === 0" class="state-container">
      <svg class="state-icon" viewBox="0 0 80 80" fill="none" xmlns="http://www.w3.org/2000/svg">
        <rect x="10" y="8" width="28" height="40" rx="3" stroke="#d1d5db" stroke-width="2" fill="#f3f4f6"/>
        <rect x="42" y="18" width="28" height="40" rx="3" stroke="#d1d5db" stroke-width="2" fill="#f9fafb"/>
        <line x1="48" y1="30" x2="64" y2="30" stroke="#e5e7eb" stroke-width="2" stroke-linecap="round"/>
        <line x1="48" y1="38" x2="60" y2="38" stroke="#e5e7eb" stroke-width="2" stroke-linecap="round"/>
        <line x1="48" y1="46" x2="56" y2="46" stroke="#e5e7eb" stroke-width="2" stroke-linecap="round"/>
        <circle cx="40" cy="60" r="8" stroke="#d1d5db" stroke-width="2" fill="none"/>
        <path d="M46 66l4 4" stroke="#d1d5db" stroke-width="2" stroke-linecap="round"/>
      </svg>
      <p class="state-title">Không tìm thấy sách phù hợp</p>
      <p class="state-hint">Hãy thử điều chỉnh lại từ khóa hoặc xóa bớt các tiêu chí lọc.</p>
      <button @click="clearFilters" class="action-btn action-btn-primary state-action">Xóa toàn bộ bộ lọc</button>
    </div>

    <div v-else class="card-grid">
      <router-link
        v-for="listing in listings"
        :key="listing.listingId"
        :to="'/listing/' + listing.listingId"
        class="book-card"
      >
        <div class="book-card-img">
          <img
            v-if="listing.images && listing.images.length > 0"
            :src="formatImageUrl(listing.images[0].imageUrl)"
            alt="Ảnh sách"
            @error="onImgError"
          />
          <div v-else class="book-card-noimg">
            <svg width="32" height="32" viewBox="0 0 24 24" fill="none"><path d="M4 19.5A2.5 2.5 0 016.5 17H20" stroke="#d1d5db" stroke-width="1.5" stroke-linecap="round"/><path d="M6.5 2H20v20H6.5A2.5 2.5 0 014 19.5v-15A2.5 2.5 0 016.5 2z" stroke="#d1d5db" stroke-width="1.5" stroke-linecap="round"/></svg>
          </div>
          <span class="book-card-badge">{{ getConditionText(listing.condition) }}</span>
          <span v-if="listing.status === 1" class="book-card-status sold">Đã bán</span>
          <span v-if="listing.status === 2" class="book-card-status removed">Đã gỡ</span>
        </div>
        <div class="book-card-body">
          <h3 class="book-card-title">{{ listing.title }}</h3>
          <p class="book-card-price">{{ listing.price.toLocaleString('vi-VN') }} đ</p>
          <p class="book-card-seller" v-if="listing.seller">{{ listing.seller.fullName }}</p>
        </div>
      </router-link>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { getListings, getFaculties, getSubjects } from '../services/api';

const route = useRoute();
const router = useRouter();

const listings = ref([]);
const faculties = ref([]);
const subjects = ref([]);
const loading = ref(false);
const searchText = ref('');
const priceRange = ref('');

const filters = ref({
  facultyId: '',
  subjectId: '',
  condition: '',
  minPrice: '',
  maxPrice: '',
  sellerId: ''
});

const isLoggedIn = ref(!!localStorage.getItem('token'));
const user = ref(JSON.parse(localStorage.getItem('user') || '{}'));

let priceDebounce = null;

const loadFaculties = async () => {
  try {
    const res = await getFaculties();
    faculties.value = res.data;
  } catch (e) {
    console.error(e);
  }
};

const loadSubjects = async (facultyId) => {
  const id = Number(facultyId);
  if (!id) {
    subjects.value = [];
    return;
  }
  try {
    const res = await getSubjects(id);
    subjects.value = res.data;
  } catch (e) {
    console.error(e);
  }
};

const onFacultyChange = () => {
  filters.value.subjectId = '';
  applyFiltersNow();
};

const applySearch = () => {
  applyFiltersNow();
};

const applyFiltersNow = () => {
  const query = {};
  if (searchText.value) query.search = searchText.value;
  if (filters.value.facultyId) query.facultyId = filters.value.facultyId;
  if (filters.value.subjectId) query.subjectId = filters.value.subjectId;
  if (filters.value.condition) query.condition = filters.value.condition;
  if (filters.value.minPrice) query.minPrice = filters.value.minPrice;
  if (filters.value.maxPrice) query.maxPrice = filters.value.maxPrice;
  if (route.query.sellerId) query.sellerId = route.query.sellerId;
  router.push({ path: '/', query });
};

const clearFilters = () => {
  searchText.value = '';
  priceRange.value = '';
  filters.value = {
    facultyId: '',
    subjectId: '',
    condition: '',
    minPrice: '',
    maxPrice: '',
    sellerId: ''
  };
  const query = {};
  if (route.query.sellerId) {
    query.sellerId = route.query.sellerId;
  }
  router.push({ path: '/', query });
};

const fetchListings = async () => {
  loading.value = true;
  try {
    const q = { ...route.query };

    if (q.search) searchText.value = q.search;
    else searchText.value = '';

    if (q.facultyId) {
      filters.value.facultyId = Number(q.facultyId);
      await loadSubjects(q.facultyId);
    } else {
      filters.value.facultyId = '';
      subjects.value = [];
    }

    if (q.subjectId) filters.value.subjectId = Number(q.subjectId);
    else filters.value.subjectId = '';

    if (q.condition) filters.value.condition = Number(q.condition);
    else filters.value.condition = '';
    
    if (q.minPrice || q.maxPrice) {
      filters.value.minPrice = q.minPrice || '';
      filters.value.maxPrice = q.maxPrice || '';
      if (q.minPrice === '0' && q.maxPrice === '20000') priceRange.value = '0-20000';
      else if (q.minPrice === '20000' && q.maxPrice === '50000') priceRange.value = '20000-50000';
      else if (q.minPrice === '50000' && q.maxPrice === '100000') priceRange.value = '50000-100000';
      else if (q.minPrice === '100000' && q.maxPrice === '200000') priceRange.value = '100000-200000';
      else if (q.minPrice === '200000' && !q.maxPrice) priceRange.value = '200000-';
      else priceRange.value = '';
    } else {
      priceRange.value = '';
    }

    const response = await getListings(q);
    listings.value = response.data;
  } catch (e) {
    console.error(e);
  } finally {
    loading.value = false;
  }
};

const getConditionText = (condition) => {
  switch (condition) {
    case 0: return 'Sách gốc - Mới';
    case 1: return 'Sách gốc - Đã qua sử dụng';
    case 2: return 'Sách photo - Mới';
    case 3: return 'Sách photo - Đã qua sử dụng';
    default: return '';
  }
};

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

watch(() => filters.value.condition, () => applyFiltersNow());
watch(() => filters.value.subjectId, (val) => { if (val) applyFiltersNow(); });
watch(priceRange, (val) => {
  if (val === '0-20000') { filters.value.minPrice = '0'; filters.value.maxPrice = '20000'; }
  else if (val === '20000-50000') { filters.value.minPrice = '20000'; filters.value.maxPrice = '50000'; }
  else if (val === '50000-100000') { filters.value.minPrice = '50000'; filters.value.maxPrice = '100000'; }
  else if (val === '100000-200000') { filters.value.minPrice = '100000'; filters.value.maxPrice = '200000'; }
  else if (val === '200000-') { filters.value.minPrice = '200000'; filters.value.maxPrice = ''; }
  else { filters.value.minPrice = ''; filters.value.maxPrice = ''; }
  applyFiltersNow();
});

onMounted(async () => {
  await loadFaculties();
  await loadSubjects(route.query.facultyId || '');
  await fetchListings();
});

watch(() => route.query, fetchListings, { deep: true });
</script>

<style scoped>
.listings-page {
  font-family: 'Segoe UI', -apple-system, BlinkMacSystemFont, Arial, sans-serif;
}

.page-header {
  display: flex;
  justify-content: flex-end;
  align-items: center;
  margin-bottom: 24px;
}

.page-actions {
  display: flex;
  gap: 8px;
}

.action-btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: 14px;
  font-weight: 600;
  padding: 8px 18px;
  border-radius: 8px;
  text-decoration: none;
  cursor: pointer;
  border: none;
  transition: all 0.15s;
  font-family: inherit;
}

.action-btn-primary {
  background-color: #1e40af;
  color: #ffffff;
}

.action-btn-primary:hover {
  background-color: #1e3a8a;
}

.action-btn-outline {
  background: transparent;
  color: #1e40af;
  border: 1.5px solid #1e40af;
}

.action-btn-outline:hover {
  background-color: #eff6ff;
}

.action-btn-ghost {
  background: transparent;
  color: #374151;
  border: 1.5px solid #e5e7eb;
}

.action-btn-ghost:hover {
  background-color: #f3f4f6;
}

.search-wrapper {
  margin-bottom: 20px;
}

.search-bar {
  position: relative;
  display: flex;
  align-items: center;
  background: #ffffff;
  border: 1.5px solid #d1d5db;
  border-radius: 12px;
  overflow: hidden;
  transition: border-color 0.15s, box-shadow 0.15s;
}

.search-bar:focus-within {
  border-color: #1e40af;
  box-shadow: 0 0 0 3px rgba(30, 64, 175, 0.1);
}

.search-icon {
  position: absolute;
  left: 16px;
  pointer-events: none;
}

.search-input {
  flex: 1;
  border: none;
  outline: none;
  padding: 14px 16px 14px 46px;
  font-size: 15px;
  font-family: inherit;
  color: #111827;
  background: transparent;
}

.search-input::placeholder {
  color: #9ca3af;
}

.search-btn {
  background-color: #1e40af;
  color: #ffffff;
  border: none;
  padding: 10px 24px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  margin: 4px;
  border-radius: 8px;
  font-family: inherit;
  transition: background-color 0.15s;
}

.search-btn:hover {
  background-color: #1e3a8a;
}

.filter-card {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  padding: 20px 24px;
  margin-bottom: 28px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.04);
}

.filter-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.filter-label {
  font-size: 14px;
  font-weight: 600;
  color: #6b7280;
}

.reset-btn {
  display: inline-flex;
  align-items: center;
  gap: 5px;
  background: none;
  border: none;
  color: #6b7280;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  padding: 4px 8px;
  border-radius: 6px;
  font-family: inherit;
  transition: color 0.15s, background-color 0.15s;
}

.reset-btn:hover {
  color: #dc2626;
  background-color: #fef2f2;
}

.filter-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
}

.filter-group {
  display: flex;
  flex-direction: column;
}

.filter-group label {
  font-size: 13px;
  font-weight: 600;
  color: #374151;
  margin-bottom: 6px;
}

.select-wrapper {
  position: relative;
}

.select-wrapper select {
  width: 100%;
  height: 42px;
  padding: 0 32px 0 12px;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 14px;
  font-family: inherit;
  color: #111827;
  background-color: #ffffff;
  appearance: none;
  -webkit-appearance: none;
  cursor: pointer;
  transition: border-color 0.15s;
}

.select-wrapper select:focus {
  outline: none;
  border-color: #1e40af;
  box-shadow: 0 0 0 3px rgba(30, 64, 175, 0.08);
}

.select-wrapper select:disabled {
  background-color: #f3f4f6;
  color: #9ca3af;
  cursor: not-allowed;
}

.select-arrow {
  position: absolute;
  right: 12px;
  top: 50%;
  transform: translateY(-50%);
  pointer-events: none;
}

.filter-price-group .price-inputs {
  display: flex;
  align-items: center;
  gap: 8px;
}

.price-input {
  flex: 1;
  height: 42px;
  padding: 0 12px;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 14px;
  font-family: inherit;
  color: #111827;
  min-width: 0;
  transition: border-color 0.15s;
}

.price-input:focus {
  outline: none;
  border-color: #1e40af;
  box-shadow: 0 0 0 3px rgba(30, 64, 175, 0.08);
}

.price-input::placeholder {
  color: #9ca3af;
}

.price-separator {
  color: #9ca3af;
  font-size: 14px;
  flex-shrink: 0;
}

.state-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 64px 24px;
  text-align: center;
}

.state-icon {
  width: 80px;
  height: 80px;
  margin-bottom: 20px;
}

.state-title {
  font-size: 18px;
  font-weight: 600;
  color: #374151;
  margin: 0 0 8px 0;
}

.state-hint {
  font-size: 14px;
  color: #9ca3af;
  margin: 0 0 24px 0;
}

.state-action {
  margin-top: 0;
}

.spinner {
  width: 36px;
  height: 36px;
  border: 3px solid #e5e7eb;
  border-top-color: #1e40af;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
  margin-bottom: 16px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.card-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 20px;
}

.book-card {
  display: block;
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  overflow: hidden;
  text-decoration: none;
  color: inherit;
  transition: box-shadow 0.2s, transform 0.2s;
}

.book-card:hover {
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.08);
  transform: translateY(-2px);
}

.book-card-img {
  position: relative;
  aspect-ratio: 4 / 3;
  background-color: #f3f4f6;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
}

.book-card-img img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.book-card-noimg {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 100%;
  color: #d1d5db;
}

.book-card-badge {
  position: absolute;
  top: 8px;
  left: 8px;
  padding: 3px 10px;
  border-radius: 6px;
  font-size: 11px;
  font-weight: 700;
  background-color: #1e40af;
  color: #ffffff;
  letter-spacing: 0.02em;
}

.book-card-status {
  position: absolute;
  top: 8px;
  right: 8px;
  padding: 3px 10px;
  border-radius: 6px;
  font-size: 11px;
  font-weight: 700;
}

.book-card-status.sold {
  background-color: #fef3c7;
  color: #92400e;
}

.book-card-status.removed {
  background-color: #f3f4f6;
  color: #6b7280;
}

.book-card-body {
  padding: 14px 16px 16px;
}

.book-card-title {
  font-size: 15px;
  font-weight: 600;
  color: #111827;
  margin: 0 0 8px 0;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  line-height: 1.4;
}

.book-card-price {
  font-size: 18px;
  font-weight: 700;
  color: #dc2626;
  margin: 0 0 6px 0;
}

.book-card-seller {
  font-size: 13px;
  color: #6b7280;
  margin: 0;
}

@media (max-width: 1024px) {
  .card-grid {
    grid-template-columns: repeat(3, 1fr);
  }
}

@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }
  .page-title {
    font-size: 22px;
  }
  .filter-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  .card-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 12px;
  }
}

@media (max-width: 480px) {
  .filter-grid {
    grid-template-columns: 1fr;
  }
  .card-grid {
    grid-template-columns: 1fr;
  }
  .page-actions {
    flex-wrap: wrap;
  }
}
</style>
