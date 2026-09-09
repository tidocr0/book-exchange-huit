import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5100/api'
});

api.interceptors.request.use(config => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  response => response,
  error => {
    if (error.response && error.response.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export const register = (data) => api.post('/auth/register', data);
export const login = (data) => api.post('/auth/login', data);

export const getListings = (filters = {}) => {
  const params = new URLSearchParams();
  for (const key in filters) {
    if (filters[key] !== null && filters[key] !== undefined && filters[key] !== '') {
      params.append(key, filters[key]);
    }
  }
  return api.get(`/listings?${params.toString()}`);
};

export const getListingDetail = (id) => api.get(`/listings/${id}`);

export const getFaculties = () => api.get('/faculties');
export const createFaculty = (data) => api.post('/faculties', data);
export const getSubjects = (facultyId) => {
  const params = facultyId ? `?facultyId=${facultyId}` : '';
  return api.get(`/subjects${params}`);
};
export const createSubject = (data) => api.post('/subjects', data);

export const createListing = (formData) => api.post('/listings', formData);

export const updateListing = (id, data) => api.put(`/listings/${id}`, data);

export const updateListingStatus = (id, status) => api.patch(`/listings/${id}/status`, { status });

// Availability endpoints
export const getMySchedule = () => api.get('/availability/my-schedule');
export const updateMySchedule = (slots) => api.put('/availability/my-schedule', { slots });
export const addBlackoutDate = (data) => api.post('/availability/blackout', data);
export const removeBlackoutDate = (id) => api.delete(`/availability/blackout/${id}`);
export const getAvailableDates = (sellerId) => api.get(`/availability/${sellerId}/dates`);

// Meetings endpoints
export const createMeeting = (data) => api.post('/meetings', data);
export const getMyMeetings = () => api.get('/meetings');
export const updateMeetingStatus = (id, status) => api.patch(`/meetings/${id}/status`, { status });

export const getProfile = () => api.get('/profile');
export const updateProfile = (data) => api.put('/profile', data);
export const uploadAvatar = (formData) => api.post('/profile/avatar', formData, {
  headers: { 'Content-Type': 'multipart/form-data' }
});
export const submitVerification = (formData) => api.post('/profile/verification-request', formData, {
  headers: { 'Content-Type': 'multipart/form-data' }
});

export const getAdminStats = () => api.get('/admin/stats');
export const getPendingVerifications = () => api.get('/admin/verification-requests');
export const approveVerification = (userId) => api.patch(`/admin/verification-requests/${userId}/approve`);
export const rejectVerification = (userId) => api.patch(`/admin/verification-requests/${userId}/reject`);
export const getAdminReports = () => api.get('/admin/reports');
export const resolveReport = (id, data) => api.patch(`/admin/reports/${id}/resolve`, data);
