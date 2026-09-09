<template>
  <div class="admin-layout">
    <aside class="admin-sidebar">
      <div class="sidebar-brand">
        <div class="brand-icon-box">
          <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round">
            <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z" />
          </svg>
        </div>
        <div class="brand-info">
          <h2>BookExchange</h2>
          <span class="brand-badge">Quản Trị Hệ Thống</span>
        </div>
      </div>

      <nav class="sidebar-nav">
        <div class="nav-section-heading">DANH MỤC QUẢN LÝ</div>

        <button
          type="button"
          class="sidebar-nav-item"
          :class="{ active: activeTab === 'stats' }"
          @click="switchTab('stats')"
        >
          <svg class="item-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <rect x="3" y="3" width="18" height="18" rx="2" ry="2"/>
            <rect x="7" y="15" width="4" height="4"/>
            <rect x="7" y="9" width="4" height="4"/>
            <rect x="13" y="15" width="4" height="4"/>
            <rect x="13" y="9" width="4" height="4"/>
          </svg>
          <span class="item-text">Tổng quan & Thống kê</span>
        </button>

        <button
          type="button"
          class="sidebar-nav-item"
          :class="{ active: activeTab === 'verifications' }"
          @click="switchTab('verifications')"
        >
          <svg class="item-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <rect x="3" y="5" width="18" height="14" rx="2" ry="2"/>
            <path d="M8 10h.01"/>
            <path d="M13 10h3"/>
            <path d="M13 14h3"/>
            <circle cx="8" cy="14" r="1.5"/>
          </svg>
          <span class="item-text">Duyệt xác thực thẻ</span>
          <span v-if="verifications.length > 0" class="item-badge badge-amber">
            {{ verifications.length }}
          </span>
        </button>

        <button
          type="button"
          class="sidebar-nav-item"
          :class="{ active: activeTab === 'reports' }"
          @click="switchTab('reports')"
        >
          <svg class="item-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/>
            <line x1="12" y1="9" x2="12" y2="13"/>
            <line x1="12" y1="17" x2="12.01" y2="17"/>
          </svg>
          <span class="item-text">Báo cáo vi phạm</span>
          <span v-if="reports.length > 0" class="item-badge badge-red">
            {{ reports.length }}
          </span>
        </button>
      </nav>

      <div class="sidebar-footer">
        <div class="admin-profile-pill">
          <div class="admin-avatar">AD</div>
          <div class="admin-info-text">
            <strong>Quản trị viên</strong>
            <span>admin@gmail.com</span>
          </div>
        </div>

        <router-link to="/" class="btn-return-shop">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <line x1="19" y1="12" x2="5" y2="12"/>
            <polyline points="12 19 5 12 12 5"/>
          </svg>
          <span>Về Sàn Mua Sắm</span>
        </router-link>
      </div>
    </aside>

    <main class="admin-main">
      <header class="admin-topbar">
        <div class="topbar-title-block">
          <h1 class="topbar-heading">{{ currentTabTitle }}</h1>
          <p class="topbar-subheading">{{ currentTabSubtitle }}</p>
        </div>

        <div class="topbar-actions">
          <button
            v-if="activeTab === 'verifications'"
            @click="loadVerifications"
            class="btn-topbar-action"
            :disabled="loadingVerifications"
          >
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
              <polyline points="23 4 23 10 17 10"/>
              <path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"/>
            </svg>
            Làm mới danh sách
          </button>
          <button
            v-else-if="activeTab === 'reports'"
            @click="loadReports"
            class="btn-topbar-action"
            :disabled="loadingReports"
          >
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
              <polyline points="23 4 23 10 17 10"/>
              <path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"/>
            </svg>
            Làm mới báo cáo
          </button>
          <button
            v-else
            @click="refreshStats"
            class="btn-topbar-action"
          >
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
              <polyline points="23 4 23 10 17 10"/>
              <path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"/>
            </svg>
            Cập nhật số liệu
          </button>
        </div>
      </header>

      <div class="admin-body">
        <div v-if="activeTab === 'stats'" class="tab-pane-stats">
          <div class="stats-grid">
            <div class="stat-card">
              <div class="stat-icon-wrapper icon-users">
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
                  <circle cx="9" cy="7" r="4" />
                  <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
                  <path d="M16 3.13a4 4 0 0 1 0 7.75" />
                </svg>
              </div>
              <div class="stat-info">
                <span class="stat-label">Tổng thành viên</span>
                <span class="stat-value">{{ stats.totalUsers }}</span>
              </div>
            </div>

            <div class="stat-card">
              <div class="stat-icon-wrapper icon-listings">
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20" />
                  <path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z" />
                </svg>
              </div>
              <div class="stat-info">
                <span class="stat-label">Tổng tin đăng</span>
                <span class="stat-value">{{ stats.totalListings }}</span>
              </div>
            </div>

            <div class="stat-card">
              <div class="stat-icon-wrapper icon-sold">
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <polyline points="20 6 9 17 4 12" />
                </svg>
              </div>
              <div class="stat-info">
                <span class="stat-label">Sách đã bán</span>
                <span class="stat-value">{{ stats.totalListingsSold }}</span>
              </div>
            </div>

            <div class="stat-card">
              <div class="stat-icon-wrapper icon-meetings">
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <rect x="3" y="4" width="18" height="18" rx="2" ry="2" />
                  <line x1="16" y1="2" x2="16" y2="6" />
                  <line x1="8" y1="2" x2="8" y2="6" />
                  <line x1="3" y1="10" x2="21" y2="10" />
                </svg>
              </div>
              <div class="stat-info">
                <span class="stat-label">Giao dịch xong</span>
                <span class="stat-value">{{ stats.totalMeetingsCompleted }}</span>
              </div>
            </div>

            <div
              class="stat-card stat-clickable highlight-amber"
              @click="switchTab('verifications')"
            >
              <div class="stat-icon-wrapper icon-pending-v">
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <rect x="3" y="5" width="18" height="14" rx="2" ry="2"/>
                  <path d="M8 10h.01"/>
                  <path d="M13 10h3"/>
                  <path d="M13 14h3"/>
                  <circle cx="8" cy="14" r="1.5"/>
                </svg>
              </div>
              <div class="stat-info">
                <span class="stat-label">Chờ xác thực</span>
                <span class="stat-value">{{ stats.pendingVerifications }}</span>
                <span class="stat-hint">Mở danh sách duyệt &rarr;</span>
              </div>
            </div>

            <div
              class="stat-card stat-clickable highlight-red"
              @click="switchTab('reports')"
            >
              <div class="stat-icon-wrapper icon-pending-r">
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z" />
                  <line x1="12" y1="9" x2="12" y2="13" />
                  <line x1="12" y1="17" x2="12.01" y2="17" />
                </svg>
              </div>
              <div class="stat-info">
                <span class="stat-label">Báo cáo chờ xử lý</span>
                <span class="stat-value">{{ stats.pendingReports }}</span>
                <span class="stat-hint">Mở danh sách xử lý &rarr;</span>
              </div>
            </div>
          </div>
        </div>

        <div v-if="activeTab === 'verifications'" class="tab-pane-verifications">
          <div v-if="loadingVerifications" class="loading-state">
            <div class="spinner"></div>
            <p>Đang tải danh sách chờ duyệt...</p>
          </div>

          <div v-else-if="verifications.length === 0" class="empty-state">
            <div class="empty-icon">🪪</div>
            <h3>Không có yêu cầu xác thực</h3>
            <p>Tất cả thẻ sinh viên đều đã được kiểm duyệt hoặc chưa có yêu cầu mới.</p>
          </div>

          <div v-else class="table-responsive">
            <table class="data-table">
              <thead>
                <tr>
                  <th>Thành viên</th>
                  <th>Ảnh thẻ sinh viên</th>
                  <th>Thao tác</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="item in verifications" :key="item.userId">
                  <td class="col-user">
                    <div class="user-info-group">
                      <strong>{{ item.fullName }}</strong>
                      <span class="user-email">{{ item.email }}</span>
                    </div>
                  </td>
                  <td class="col-images">
                    <div class="card-images-wrapper">
                      <div
                        v-if="item.studentIdFrontUrl"
                        class="card-img-thumb"
                        @click="zoomImage(formatImageUrl(item.studentIdFrontUrl), 'Mặt trước: ' + item.fullName)"
                      >
                        <img :src="formatImageUrl(item.studentIdFrontUrl)" alt="Mặt trước" loading="lazy" />
                        <div class="thumb-overlay">
                          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                            <circle cx="11" cy="11" r="8"/>
                            <line x1="21" y1="21" x2="16.65" y2="16.65"/>
                            <line x1="11" y1="8" x2="11" y2="14"/>
                            <line x1="8" y1="11" x2="14" y2="11"/>
                          </svg>
                        </div>
                        <span class="thumb-tag">Mặt trước</span>
                      </div>
                      <div
                        v-if="item.studentIdBackUrl"
                        class="card-img-thumb"
                        @click="zoomImage(formatImageUrl(item.studentIdBackUrl), 'Mặt sau: ' + item.fullName)"
                      >
                        <img :src="formatImageUrl(item.studentIdBackUrl)" alt="Mặt sau" loading="lazy" />
                        <div class="thumb-overlay">
                          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                            <circle cx="11" cy="11" r="8"/>
                            <line x1="21" y1="21" x2="16.65" y2="16.65"/>
                            <line x1="11" y1="8" x2="11" y2="14"/>
                            <line x1="8" y1="11" x2="14" y2="11"/>
                          </svg>
                        </div>
                        <span class="thumb-tag">Mặt sau</span>
                      </div>
                    </div>
                  </td>
                  <td class="col-actions">
                    <button @click="handleApprove(item.userId)" class="btn btn-approve">
                      ✓ Duyệt
                    </button>
                    <button @click="handleReject(item.userId)" class="btn btn-reject">
                      ✕ Từ chối
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <div v-if="activeTab === 'reports'" class="tab-pane-reports">
          <div v-if="loadingReports" class="loading-state">
            <div class="spinner"></div>
            <p>Đang tải danh sách báo cáo...</p>
          </div>

          <div v-else-if="reports.length === 0" class="empty-state">
            <div class="empty-icon">⚠️</div>
            <h3>Không có báo cáo vi phạm</h3>
            <p>Hệ thống hiện tại chưa có báo cáo nào cần xử lý.</p>
          </div>

          <div v-else class="table-responsive">
            <table class="data-table">
              <thead>
                <tr>
                  <th>Người báo cáo</th>
                  <th>Lý do vi phạm</th>
                  <th>Đối tượng bị báo cáo</th>
                  <th>Thời gian</th>
                  <th>Thao tác</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="r in reports" :key="r.reportId">
                  <td class="col-reporter">
                    <strong>{{ r.reporterFullName }}</strong>
                  </td>
                  <td class="col-reason">
                    {{ r.reason }}
                  </td>
                  <td class="col-target">
                    <div v-if="r.listingTitle">
                      <span class="target-badge">Tin đăng:</span> {{ r.listingTitle }}
                    </div>
                    <div v-if="r.reportedUserFullName">
                      <span class="target-badge">Tài khoản:</span> {{ r.reportedUserFullName }}
                    </div>
                  </td>
                  <td class="col-date">
                    {{ formatDate(r.createdAt) }}
                  </td>
                  <td class="col-actions">
                    <button @click="openResolveModal(r)" class="btn btn-resolve">
                      Xử lý
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </main>

    <div v-if="activeReport" class="modal-overlay" @click.self="closeResolveModal">
      <div class="modal-card">
        <div class="modal-header">
          <h3>Xử lý báo cáo #{{ activeReport.reportId }}</h3>
          <button @click="closeResolveModal" class="btn-close">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <line x1="18" y1="6" x2="6" y2="18"/>
              <line x1="6" y1="6" x2="18" y2="18"/>
            </svg>
          </button>
        </div>

        <div class="modal-body">
          <div class="report-summary">
            <p><strong>Người báo cáo:</strong> {{ activeReport.reporterFullName }}</p>
            <p><strong>Lý do:</strong> {{ activeReport.reason }}</p>
            <p v-if="activeReport.listingTitle"><strong>Tin đăng:</strong> {{ activeReport.listingTitle }}</p>
            <p v-if="activeReport.reportedUserFullName"><strong>Tài khoản bị báo cáo:</strong> {{ activeReport.reportedUserFullName }}</p>
          </div>

          <label class="modal-label">Chọn hành động xử lý:</label>
          <div class="action-options">
            <label class="action-option-item" :class="{ selected: selectedAction === 1 }">
              <input type="radio" :value="1" v-model="selectedAction" />
              <div>
                <strong>1. Ẩn tin đăng vi phạm</strong>
                <p>Tin đăng này sẽ bị chuyển sang trạng thái Đã gỡ và không hiển thị cho người mua.</p>
              </div>
            </label>

            <label class="action-option-item" :class="{ selected: selectedAction === 2 }">
              <input type="radio" :value="2" v-model="selectedAction" />
              <div>
                <strong>2. Khoá tài khoản người dùng</strong>
                <p>Tài khoản vi phạm sẽ bị vô hiệu hóa quyền đăng nhập vào hệ thống.</p>
              </div>
            </label>

            <label class="action-option-item" :class="{ selected: selectedAction === 3 }">
              <input type="radio" :value="3" v-model="selectedAction" />
              <div>
                <strong>3. Bỏ qua & Đóng báo cáo</strong>
                <p>Nội dung hợp lệ hoặc không đủ cơ sở vi phạm, chỉ đóng báo cáo này.</p>
              </div>
            </label>
          </div>
        </div>

        <div class="modal-footer">
          <button @click="closeResolveModal" class="btn btn-cancel">Hủy</button>
          <button @click="submitResolve" class="btn btn-primary-action" :disabled="submittingAction">
            <span v-if="submittingAction">Đang lưu...</span>
            <span v-else>Xác nhận xử lý</span>
          </button>
        </div>
      </div>
    </div>

    <div v-if="zoomedImage" class="modal-overlay" @click="zoomedImage = null">
      <div class="zoom-modal-card" @click.stop>
        <div class="zoom-header">
          <span>{{ zoomedImageTitle }}</span>
          <button @click="zoomedImage = null" class="btn-close">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <line x1="18" y1="6" x2="6" y2="18"/>
              <line x1="6" y1="6" x2="18" y2="18"/>
            </svg>
          </button>
        </div>
        <img :src="zoomedImage" alt="Zoomed card" class="zoomed-img" />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import {
  getAdminStats,
  getPendingVerifications,
  approveVerification,
  rejectVerification,
  getAdminReports,
  resolveReport
} from '../services/api';
import { useToast } from '../composables/useToast';

const route = useRoute();
const router = useRouter();
const { showToast } = useToast();

const activeTab = ref(route.query.tab || 'stats');

const currentTabTitle = computed(() => {
  if (activeTab.value === 'verifications') return 'Duyệt Xác Thực Thẻ Sinh Viên';
  if (activeTab.value === 'reports') return 'Xử Lý Báo Cáo Vi Phạm';
  return 'Tổng Quan & Số Liệu Thống Kê';
});

const currentTabSubtitle = computed(() => {
  if (activeTab.value === 'verifications') return 'Kiểm tra và phê duyệt ảnh thẻ sinh viên của các thành viên';
  if (activeTab.value === 'reports') return 'Xem xét khiếu nại và thực hiện biện pháp xử lý vi phạm';
  return 'Theo dõi hoạt động giao dịch, tài khoản và tin đăng trên toàn sàn';
});

const switchTab = (tab) => {
  activeTab.value = tab;
  router.replace({ query: { ...route.query, tab } });
};

const stats = ref({
  totalUsers: 0,
  totalListings: 0,
  totalListingsSold: 0,
  totalMeetingsCompleted: 0,
  pendingVerifications: 0,
  pendingReports: 0
});

const verifications = ref([]);
const reports = ref([]);

const loadingVerifications = ref(false);
const loadingReports = ref(false);

const activeReport = ref(null);
const selectedAction = ref(1);
const submittingAction = ref(false);

const zoomedImage = ref(null);
const zoomedImageTitle = ref('');

const loadStats = async () => {
  try {
    const res = await getAdminStats();
    stats.value = res.data;
  } catch (err) {
    showToast(err.response?.data?.message || 'Không thể tải số liệu thống kê.', 'error');
  }
};

const refreshStats = async () => {
  await loadStats();
  await loadVerifications();
  await loadReports();
  showToast('Đã làm mới toàn bộ số liệu thống kê.', 'success');
};

const loadVerifications = async () => {
  loadingVerifications.value = true;
  try {
    const res = await getPendingVerifications();
    verifications.value = res.data || [];
  } catch (err) {
    showToast(err.response?.data?.message || 'Không thể tải danh sách xác thực.', 'error');
  } finally {
    loadingVerifications.value = false;
  }
};

const loadReports = async () => {
  loadingReports.value = true;
  try {
    const res = await getAdminReports();
    reports.value = res.data || [];
  } catch (err) {
    showToast(err.response?.data?.message || 'Không thể tải danh sách báo cáo.', 'error');
  } finally {
    loadingReports.value = false;
  }
};

const handleApprove = async (userId) => {
  try {
    await approveVerification(userId);
    showToast('Đã duyệt xác thực thẻ sinh viên thành công!', 'success');
    verifications.value = verifications.value.filter(v => v.userId !== userId);
    stats.value.pendingVerifications = Math.max(0, stats.value.pendingVerifications - 1);
  } catch (err) {
    showToast(err.response?.data?.message || 'Có lỗi khi duyệt xác thực.', 'error');
  }
};

const handleReject = async (userId) => {
  try {
    await rejectVerification(userId);
    showToast('Đã từ chối xác thực thẻ sinh viên.', 'success');
    verifications.value = verifications.value.filter(v => v.userId !== userId);
    stats.value.pendingVerifications = Math.max(0, stats.value.pendingVerifications - 1);
  } catch (err) {
    showToast(err.response?.data?.message || 'Có lỗi khi từ chối xác thực.', 'error');
  }
};

const openResolveModal = (report) => {
  activeReport.value = report;
  selectedAction.value = 1;
};

const closeResolveModal = () => {
  activeReport.value = null;
  submittingAction.value = false;
};

const submitResolve = async () => {
  if (!activeReport.value) return;

  submittingAction.value = true;
  try {
    await resolveReport(activeReport.value.reportId, { action: selectedAction.value });
    showToast('Đã xử lý báo cáo thành công!', 'success');
    reports.value = reports.value.filter(r => r.reportId !== activeReport.value.reportId);
    stats.value.pendingReports = Math.max(0, stats.value.pendingReports - 1);
    closeResolveModal();
  } catch (err) {
    showToast(err.response?.data?.message || 'Có lỗi khi xử lý báo cáo.', 'error');
  } finally {
    submittingAction.value = false;
  }
};

const zoomImage = (url, title) => {
  zoomedImage.value = url;
  zoomedImageTitle.value = title;
};

const formatImageUrl = (url) => {
  if (!url) return '';
  if (url.startsWith('http://') || url.startsWith('https://')) return url;
  return 'http://localhost:5100' + url;
};

const formatDate = (isoStr) => {
  if (!isoStr) return '';
  const d = new Date(isoStr);
  return d.toLocaleDateString('vi-VN') + ' ' + d.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
};

onMounted(() => {
  loadStats();
  loadVerifications();
  loadReports();
});
</script>

<style scoped>
.admin-layout {
  display: flex;
  min-height: 100vh;
  background-color: #f8fafc;
  color: #0f172a;
  font-family: inherit;
}

.admin-sidebar {
  width: 280px;
  background-color: #ffffff;
  border-right: 1px solid #e2e8f0;
  display: flex;
  flex-direction: column;
  position: sticky;
  top: 0;
  height: 100vh;
  flex-shrink: 0;
  z-index: 50;
}

.sidebar-brand {
  padding: 24px;
  display: flex;
  align-items: center;
  gap: 14px;
  border-bottom: 1px solid #f1f5f9;
}

.brand-icon-box {
  width: 44px;
  height: 44px;
  border-radius: 12px;
  background-color: #eff6ff;
  color: #2563eb;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  box-shadow: inset 0 0 0 1px #dbeafe;
}

.brand-info h2 {
  font-size: 16px;
  font-weight: 700;
  color: #0f172a;
  margin: 0;
  line-height: 1.2;
}

.brand-badge {
  font-size: 12px;
  color: #64748b;
  font-weight: 500;
  display: block;
  margin-top: 4px;
}

.sidebar-nav {
  padding: 24px 16px;
  display: flex;
  flex-direction: column;
  gap: 8px;
  flex: 1;
  overflow-y: auto;
}

.nav-section-heading {
  font-size: 11px;
  font-weight: 700;
  color: #94a3b8;
  letter-spacing: 0.05em;
  padding: 0 12px 8px 12px;
  text-transform: uppercase;
}

.sidebar-nav-item {
  display: flex;
  align-items: center;
  gap: 14px;
  padding: 14px 16px;
  border-radius: 12px;
  background: none;
  border: 1px solid transparent;
  color: #475569;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  text-align: left;
  width: 100%;
}

.sidebar-nav-item:hover {
  background-color: #f8fafc;
  color: #0f172a;
}

.sidebar-nav-item.active {
  background-color: #eff6ff;
  color: #1d4ed8;
  font-weight: 600;
}

.item-icon {
  width: 20px;
  height: 20px;
  flex-shrink: 0;
  color: inherit;
  opacity: 0.7;
}

.sidebar-nav-item.active .item-icon {
  opacity: 1;
}

.item-text {
  flex: 1;
}

.item-badge {
  font-size: 11px;
  font-weight: 700;
  padding: 3px 8px;
  border-radius: 12px;
  line-height: 1;
}

.badge-amber {
  background-color: #fffbeb;
  color: #d97706;
  border: 1px solid #fde68a;
}

.badge-red {
  background-color: #fef2f2;
  color: #dc2626;
  border: 1px solid #fecaca;
}

.sidebar-footer {
  padding: 20px 16px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.admin-profile-pill {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 10px 12px;
  border-radius: 12px;
}

.admin-avatar {
  width: 36px;
  height: 36px;
  border-radius: 10px;
  background-color: #f1f5f9;
  color: #334155;
  font-size: 12px;
  font-weight: 700;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  border: 1px solid #e2e8f0;
}

.admin-info-text {
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.admin-info-text strong {
  font-size: 13px;
  color: #0f172a;
}

.admin-info-text span {
  font-size: 12px;
  color: #64748b;
  text-overflow: ellipsis;
  overflow: hidden;
  white-space: nowrap;
}

.btn-return-shop {
  background-color: #ffffff;
  color: #334155;
  padding: 12px 16px;
  border-radius: 10px;
  font-size: 13px;
  font-weight: 600;
  text-decoration: none;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  border: 1px solid #e2e8f0;
  transition: all 0.2s ease;
}

.btn-return-shop:hover {
  background-color: #f8fafc;
  color: #0f172a;
  border-color: #cbd5e1;
}

.admin-main {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.admin-topbar {
  background-color: #f8fafc;
  padding: 32px 40px 16px 40px;
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
}

.topbar-title-block h1 {
  font-size: 24px;
  font-weight: 700;
  color: #0f172a;
  margin: 0;
  letter-spacing: -0.01em;
}

.topbar-subheading {
  font-size: 14px;
  color: #64748b;
  margin-top: 6px;
  margin-bottom: 0;
}

.btn-topbar-action {
  display: flex;
  align-items: center;
  gap: 8px;
  background-color: #ffffff;
  border: 1px solid #e2e8f0;
  color: #334155;
  font-size: 13px;
  font-weight: 600;
  padding: 10px 16px;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.2s ease;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.02);
}

.btn-topbar-action:hover:not(:disabled) {
  background-color: #f1f5f9;
  border-color: #cbd5e1;
  color: #0f172a;
}

.admin-body {
  padding: 24px 40px 60px 40px;
  flex: 1;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
  gap: 20px;
  margin-bottom: 24px;
}

.stat-card {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 16px;
  padding: 24px;
  display: flex;
  align-items: center;
  gap: 20px;
  transition: all 0.2s ease;
}

.stat-clickable {
  cursor: pointer;
}

.stat-clickable:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.05), 0 8px 10px -6px rgba(0, 0, 0, 0.01);
}

.stat-icon-wrapper {
  width: 52px;
  height: 52px;
  border-radius: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.icon-users { background-color: #eff6ff; color: #2563eb; }
.icon-listings { background-color: #f0fdf4; color: #16a34a; }
.icon-sold { background-color: #fdf4ff; color: #c026d3; }
.icon-meetings { background-color: #f8fafc; color: #475569; }
.icon-pending-v { background-color: #fffbeb; color: #d97706; }
.icon-pending-r { background-color: #fef2f2; color: #dc2626; }

.stat-card.highlight-amber {
  border-color: #fcd34d;
  background-color: #fffdf5;
}

.stat-card.highlight-red {
  border-color: #fca5a5;
  background-color: #fffafa;
}

.stat-info {
  display: flex;
  flex-direction: column;
}

.stat-label {
  font-size: 13px;
  color: #64748b;
  font-weight: 500;
  margin-bottom: 4px;
}

.stat-value {
  font-size: 28px;
  font-weight: 700;
  color: #0f172a;
  line-height: 1;
}

.stat-hint {
  font-size: 12px;
  color: #2563eb;
  font-weight: 500;
  margin-top: 6px;
}

.table-responsive {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.02);
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  text-align: left;
}

.data-table th {
  background-color: #f8fafc;
  color: #475569;
  font-weight: 600;
  padding: 16px 24px;
  border-bottom: 1px solid #e2e8f0;
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.data-table td {
  padding: 20px 24px;
  border-bottom: 1px solid #f1f5f9;
  vertical-align: middle;
}

.data-table tbody tr:last-child td {
  border-bottom: none;
}

.data-table tbody tr:hover {
  background-color: #f8fafc;
}

.user-info-group {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.col-user strong {
  color: #0f172a;
  font-size: 14px;
}

.user-email {
  font-size: 13px;
  color: #64748b;
}

.col-reporter strong {
  color: #0f172a;
  font-size: 14px;
}

.col-reason {
  color: #334155;
  font-size: 14px;
  line-height: 1.5;
  max-width: 320px;
}

.target-badge {
  font-size: 12px;
  color: #64748b;
  font-weight: 500;
}

.col-date {
  color: #64748b;
  font-size: 13px;
}

.card-images-wrapper {
  display: flex;
  gap: 16px;
}

.card-img-thumb {
  position: relative;
  width: 140px;
  height: 88px;
  border-radius: 8px;
  overflow: hidden;
  border: 1px solid #e2e8f0;
  cursor: zoom-in;
  background-color: #f1f5f9;
}

.card-img-thumb img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.card-img-thumb:hover img {
  transform: scale(1.05);
}

.thumb-overlay {
  position: absolute;
  inset: 0;
  background-color: rgba(0, 0, 0, 0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  color: #ffffff;
  opacity: 0;
  transition: opacity 0.2s ease;
}

.card-img-thumb:hover .thumb-overlay {
  opacity: 1;
}

.thumb-tag {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  background: linear-gradient(transparent, rgba(0, 0, 0, 0.7));
  color: #ffffff;
  font-size: 11px;
  font-weight: 600;
  padding: 16px 8px 6px 8px;
  pointer-events: none;
}

.col-actions {
  display: flex;
  gap: 10px;
}

.btn {
  padding: 8px 16px;
  border-radius: 8px;
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  border: none;
  transition: all 0.2s ease;
}

.btn-approve {
  background-color: #10b981;
  color: #ffffff;
}

.btn-approve:hover {
  background-color: #059669;
}

.btn-reject {
  background-color: #ef4444;
  color: #ffffff;
}

.btn-reject:hover {
  background-color: #dc2626;
}

.btn-resolve {
  background-color: #ffffff;
  border: 1px solid #cbd5e1;
  color: #0f172a;
}

.btn-resolve:hover {
  background-color: #f1f5f9;
  border-color: #94a3b8;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 60px 20px;
  gap: 16px;
  color: #64748b;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 80px 20px;
  text-align: center;
  background: #ffffff;
  border: 1px dashed #cbd5e1;
  border-radius: 16px;
}

.empty-icon {
  font-size: 48px;
  margin-bottom: 16px;
  opacity: 0.5;
}

.empty-state h3 {
  font-size: 18px;
  font-weight: 600;
  color: #0f172a;
  margin: 0 0 8px 0;
}

.empty-state p {
  font-size: 14px;
  color: #64748b;
  margin: 0;
}

.spinner {
  width: 32px;
  height: 32px;
  border: 3px solid #e2e8f0;
  border-top-color: #2563eb;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.modal-overlay {
  position: fixed;
  inset: 0;
  background-color: rgba(15, 23, 42, 0.6);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 20px;
  backdrop-filter: blur(4px);
}

.modal-card {
  background: #ffffff;
  border-radius: 16px;
  max-width: 540px;
  width: 100%;
  overflow: hidden;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
}

.modal-header {
  padding: 20px 24px;
  border-bottom: 1px solid #e2e8f0;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.modal-header h3 {
  font-size: 16px;
  font-weight: 700;
  color: #0f172a;
  margin: 0;
}

.btn-close {
  background: none;
  border: none;
  color: #64748b;
  cursor: pointer;
  padding: 4px;
  border-radius: 6px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
}

.btn-close:hover {
  background-color: #f1f5f9;
  color: #0f172a;
}

.modal-body {
  padding: 24px;
}

.report-summary {
  background-color: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 16px;
  margin-bottom: 24px;
  font-size: 13px;
  display: flex;
  flex-direction: column;
  gap: 6px;
  color: #334155;
}

.report-summary strong {
  color: #0f172a;
}

.modal-label {
  font-size: 14px;
  font-weight: 600;
  color: #0f172a;
  display: block;
  margin-bottom: 12px;
}

.action-options {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.action-option-item {
  display: flex;
  align-items: flex-start;
  gap: 16px;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 16px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.action-option-item:hover {
  border-color: #cbd5e1;
  background-color: #f8fafc;
}

.action-option-item.selected {
  border-color: #2563eb;
  background-color: #eff6ff;
  box-shadow: 0 0 0 1px #2563eb;
}

.action-option-item input[type="radio"] {
  margin-top: 2px;
}

.action-option-item strong {
  display: block;
  font-size: 14px;
  color: #0f172a;
  margin-bottom: 4px;
}

.action-option-item p {
  font-size: 13px;
  color: #64748b;
  margin: 0;
  line-height: 1.5;
}

.modal-footer {
  padding: 16px 24px;
  background-color: #f8fafc;
  border-top: 1px solid #e2e8f0;
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.btn-cancel {
  background: #ffffff;
  border: 1px solid #cbd5e1;
  color: #334155;
}

.btn-cancel:hover {
  background-color: #f1f5f9;
}

.btn-primary-action {
  background-color: #2563eb;
  color: #ffffff;
}

.btn-primary-action:hover:not(:disabled) {
  background-color: #1d4ed8;
}

.zoom-modal-card {
  background: #ffffff;
  border-radius: 12px;
  max-width: 800px;
  width: 100%;
  overflow: hidden;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
  display: flex;
  flex-direction: column;
}

.zoom-header {
  padding: 16px 24px;
  background-color: #ffffff;
  color: #0f172a;
  border-bottom: 1px solid #e2e8f0;
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 15px;
  font-weight: 600;
}

.zoomed-img {
  width: 100%;
  max-height: 80vh;
  object-fit: contain;
  background-color: #f1f5f9;
}

@media (max-width: 900px) {
  .admin-layout {
    flex-direction: column;
  }
  .admin-sidebar {
    width: 100%;
    height: auto;
    position: static;
    border-right: none;
    border-bottom: 1px solid #e2e8f0;
  }
  .sidebar-nav {
    flex-direction: row;
    overflow-x: auto;
    padding: 16px;
  }
  .nav-section-heading, .sidebar-footer {
    display: none;
  }
  .sidebar-nav-item {
    width: auto;
    white-space: nowrap;
  }
  .admin-topbar {
    padding: 24px;
    flex-direction: column;
    align-items: flex-start;
    gap: 16px;
  }
  .admin-body {
    padding: 24px;
  }
}
</style>
