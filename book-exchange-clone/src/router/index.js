import { createRouter, createWebHistory } from 'vue-router';
import ListingsView from '../views/ListingsView.vue';
import ListingDetailView from '../views/ListingDetailView.vue';
import LoginView from '../views/LoginView.vue';
import RegisterView from '../views/RegisterView.vue';
import CreateListingView from '../views/CreateListingView.vue';
import ScheduleView from '../views/ScheduleView.vue';
import MeetingsView from '../views/MeetingsView.vue';
import ProfileView from '../views/ProfileView.vue';
import AdminView from '../views/AdminView.vue';

const routes = [
  { path: '/', component: ListingsView },
  { path: '/listing/:id', component: ListingDetailView },
  { path: '/login', component: LoginView },
  { path: '/register', component: RegisterView },
  { path: '/create', component: CreateListingView },
  { path: '/schedule', component: ScheduleView },
  { path: '/meetings', component: MeetingsView },
  { path: '/profile', component: ProfileView },
  { path: '/admin', component: AdminView }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

router.beforeEach((to, from, next) => {
  if (to.path.startsWith('/admin')) {
    const userStr = localStorage.getItem('user');
    let isAdmin = false;
    if (userStr) {
      try {
        const user = JSON.parse(userStr);
        isAdmin = user.isAdmin === true;
      } catch (e) {
        isAdmin = false;
      }
    }
    if (!isAdmin) {
      return next('/');
    }
  }
  next();
});

export default router;
