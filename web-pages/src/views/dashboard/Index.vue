<template>
  <div>
    <h2>ダッシュボード</h2>
    <div class="dashboard">
      <div
        v-for="(menu, index) in menuList"
        :key="index"
        class="card-container"
      >
        <router-link :to="menu.url">
          <el-card class="menu-card" shadow="hover">
            <div class="card-accent" />
            <div class="card-content">
              <div class="menu-header">
                <span class="menu-icon-wrapper">
                  <icon
                    v-if="menu.category"
                    :name="menu.category"
                    scale="1.5"
                    class="menu-icon"
                  />
                </span>
                <span class="menu-name">{{ menu.name }}</span>
              </div>
              <div class="menu-description">
                {{ menu.description }}
              </div>
              <div class="card-arrow">
                <i class="el-icon-arrow-right" />
              </div>
            </div>
          </el-card>
        </router-link>
      </div>
    </div>
    <div class="footer">
      <span class="footer-content">
        &copy; 2016-2020 NS Solutions Corporation, All Rights Reserved.
      </span>
    </div>
  </div>
</template>

<script>
import { createNamespacedHelpers } from 'vuex'
const { mapGetters } = createNamespacedHelpers('account')

export default {
  title: 'ダッシュボード',
  data() {
    return {}
  },
  computed: {
    ...mapGetters(['menuList']),
  },
}
</script>

<style lang="scss" scoped>
$primary: #4f46e5;
$primary-light: #6366f1;
$primary-bg: #eef2ff;
$text-primary: #1e293b;
$text-secondary: #64748b;
$border-color: #e2e8f0;

a {
  text-decoration: none;
}

.dashboard {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(340px, 1fr));
  gap: 20px;
  padding: 4px;
}

.card-container {
  display: flex;
}

.menu-card {
  width: 100%;
  border: 1px solid $border-color !important;
  border-radius: 12px !important;
  overflow: hidden;
  position: relative;
  transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1) !important;
  cursor: pointer;

  &:hover {
    transform: translateY(-4px) !important;
    box-shadow: 0 10px 25px -5px rgba(79, 70, 229, 0.15),
      0 4px 10px -5px rgba(79, 70, 229, 0.1) !important;
    border-color: lighten($primary, 25%) !important;

    .card-accent {
      width: 6px;
      background: linear-gradient(180deg, $primary 0%, $primary-light 100%);
    }

    .card-arrow {
      opacity: 1;
      transform: translateX(0);
    }

    .menu-icon-wrapper {
      background-color: $primary;

      .menu-icon {
        color: #fff !important;
      }
    }
  }
}

.card-accent {
  position: absolute;
  left: 0;
  top: 0;
  bottom: 0;
  width: 4px;
  background-color: $primary;
  transition: all 0.25s ease;
  border-radius: 0 4px 4px 0;
}

.card-content {
  padding: 4px 8px 4px 12px;
  position: relative;
}

.menu-header {
  display: flex;
  align-items: center;
  margin-bottom: 12px;
}

.menu-icon-wrapper {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  border-radius: 10px;
  background-color: $primary-bg;
  margin-right: 14px;
  flex-shrink: 0;
  transition: all 0.25s ease;
}

.menu-icon {
  color: $primary !important;
  transition: color 0.25s ease;
}

.menu-name {
  font-weight: 600;
  font-size: 17px;
  color: $text-primary;
  letter-spacing: -0.01em;
}

.menu-description {
  font-size: 13px;
  color: $text-secondary;
  line-height: 1.6;
  padding-left: 54px;
}

.card-arrow {
  position: absolute;
  right: 8px;
  top: 50%;
  transform: translateX(-8px);
  opacity: 0;
  transition: all 0.25s ease;
  color: $primary;
  font-size: 16px;
  margin-top: -8px;
}

.footer {
  display: flex;
  justify-content: center;
  margin-top: 48px;
}

.footer-content {
  color: $text-secondary;
  font-size: 12px;
  padding: 16px;
}
</style>
