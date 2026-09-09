<template>
  <div class="auth-page">
    <div class="auth-card" style="max-width: 600px;">
      <h2 class="auth-title">Đăng tin bán sách mới</h2>
      <form @submit.prevent="handleSubmit" class="auth-form">
        <div class="form-group">
          <label for="create-title">Tiêu đề</label>
          <input id="create-title" type="text" v-model="form.title" required class="form-input" />
        </div>
        <div class="form-group">
          <label for="create-desc">Mô tả</label>
          <textarea id="create-desc" v-model="form.description" required class="form-input" style="height: 100px; padding-top: 10px;"></textarea>
        </div>
        <div class="form-group">
          <label for="create-price">Giá (VNĐ)</label>
          <input id="create-price" type="number" v-model="form.price" required min="0" class="form-input" />
        </div>
        
        <div class="form-group">
          <label for="create-faculty">Khoa</label>
          <select id="create-faculty" v-model="form.facultyId" @change="onFacultyChange" required class="form-input">
            <option value="" disabled>-- Chọn khoa --</option>
            <option v-for="f in faculties" :key="f.facultyId" :value="f.facultyId">{{ f.name }}</option>
            <option value="NEW">+ Thêm khoa mới...</option>
          </select>
        </div>

        <div class="form-group" v-if="form.facultyId === 'NEW'">
          <label for="create-new-faculty">Tên khoa mới</label>
          <input id="create-new-faculty" type="text" v-model="newFacultyName" required class="form-input" placeholder="Nhập tên khoa..." />
        </div>

        <div class="form-group">
          <label for="create-subject">Môn học</label>
          <select id="create-subject" v-model="form.subjectId" :disabled="!form.facultyId" required class="form-input" @change="onSubjectChange">
            <option value="" disabled>-- Chọn môn học --</option>
            <option v-for="s in subjects" :key="s.subjectId" :value="s.subjectId">{{ s.name }}</option>
            <option value="NEW">+ Thêm môn học mới...</option>
          </select>
        </div>

        <div class="form-group" v-if="form.subjectId === 'NEW'">
          <label for="create-new-subject">Tên môn học mới</label>
          <input id="create-new-subject" type="text" v-model="newSubjectName" required class="form-input" placeholder="Nhập tên môn học..." />
        </div>

        <div class="form-group">
          <label for="create-condition">Tình trạng sách</label>
          <select id="create-condition" v-model="form.condition" required class="form-input">
            <option value="0">Sách gốc - Mới</option>
            <option value="1">Sách gốc - Đã qua sử dụng</option>
            <option value="2">Sách photo - Mới</option>
            <option value="3">Sách photo - Đã qua sử dụng</option>
          </select>
        </div>
        <div class="form-group">
          <label>Ảnh (Tối đa 5 ảnh, mỗi ảnh &lt; 5MB)</label>
          <input type="file" multiple accept=".jpg,.jpeg,.png" @change="handleFileChange" class="form-input" style="padding: 7px 12px;" />
        </div>
        
        <button type="submit" class="btn btn-primary btn-block" :disabled="loading">Đăng tin</button>
      </form>
      <div v-if="error" class="error-message">{{ error }}</div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { createListing, getFaculties, getSubjects, createSubject, createFaculty } from '../services/api';
import { useToast } from '../composables/useToast';

const { showToast } = useToast();
const router = useRouter();
const form = ref({
  title: '',
  description: '',
  price: '',
  facultyId: '',
  subjectId: '',
  condition: 0
});
const newSubjectName = ref('');
const newFacultyName = ref('');
const files = ref([]);
const loading = ref(false);
const error = ref(null);

const faculties = ref([]);
const subjects = ref([]);

const loadFaculties = async () => {
  try {
    const res = await getFaculties();
    faculties.value = res.data;
  } catch (err) {
    console.error(err);
  }
};

const onFacultyChange = async () => {
  form.value.subjectId = '';
  newSubjectName.value = '';
  subjects.value = [];
  
  if (form.value.facultyId === 'NEW') {
    newFacultyName.value = '';
    return;
  }
  
  if (form.value.facultyId) {
    try {
      const res = await getSubjects(form.value.facultyId);
      subjects.value = res.data;
    } catch (err) {
      console.error(err);
    }
  }
};

const onSubjectChange = () => {
  if (form.value.subjectId !== 'NEW') {
    newSubjectName.value = '';
  }
};

const handleFileChange = (e) => {
  files.value = Array.from(e.target.files);
};

const handleSubmit = async () => {
  loading.value = true;
  error.value = null;
  
  try {
    let finalFacultyId = form.value.facultyId;
    if (finalFacultyId === 'NEW') {
      const resFac = await createFaculty({
        name: newFacultyName.value
      });
      finalFacultyId = resFac.data.facultyId;
      // Update form so retry won't re-create the same faculty
      form.value.facultyId = finalFacultyId;
      newFacultyName.value = '';
      // Reload faculties list
      try { const r = await getFaculties(); faculties.value = r.data; } catch(_) {}
      // Load subjects for the new faculty
      try { const r = await getSubjects(finalFacultyId); subjects.value = r.data; } catch(_) {}
    }

    let finalSubjectId = form.value.subjectId;

    if (finalSubjectId === 'NEW') {
      const resSub = await createSubject({
        name: newSubjectName.value,
        facultyId: finalFacultyId
      });
      finalSubjectId = resSub.data.subjectId;
      // Update form so retry won't re-create the same subject
      form.value.subjectId = finalSubjectId;
      newSubjectName.value = '';
      // Reload subjects list
      try { const r = await getSubjects(finalFacultyId); subjects.value = r.data; } catch(_) {}
    }

    const formData = new FormData();
    formData.append('Title', form.value.title);
    formData.append('Description', form.value.description);
    formData.append('Price', form.value.price);
    formData.append('SubjectId', finalSubjectId);
    formData.append('Condition', form.value.condition);
    
    files.value.forEach(file => {
      formData.append('Images', file);
    });

    const res = await createListing(formData);
    showToast('Đăng tin thành công!', 'success');
    router.push('/listing/' + res.data.listingId);
  } catch (err) {
    error.value = err.response?.data?.message || 'Có lỗi khi đăng tin.';
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  loadFaculties();
});
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
</style>
