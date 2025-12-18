<template>
  <div class="container-fluid py-4" dir="rtl">
    <div class="row">
      <div class="col-12">
        <div class="text-center mb-4">
          <MDTypography variant="h3" font-weight="bold">
            دعم RTL
          </MDTypography>
          <MDTypography variant="body2" color="text">
            Material Dashboard يدعم الآن اللغات من اليمين إلى اليسار
          </MDTypography>
        </div>
      </div>
    </div>

    <!-- Stats Cards RTL -->
    <div class="row">
      <div class="col-xl-3 col-sm-6 mb-4">
        <div class="card">
          <div class="card-body p-3">
            <div class="row">
              <div class="col-8">
                <div class="numbers">
                  <MDTypography variant="caption" color="text" font-weight="bold" class="text-uppercase">
                    اليوم المال
                  </MDTypography>
                  <MDTypography variant="h5" font-weight="bold">
                    53,000 ر.س
                  </MDTypography>
                  <MDTypography variant="caption" color="success" font-weight="bold">
                    <Icon name="mdi:arrow-up" size="16" />
                    55%
                  </MDTypography>
                </div>
              </div>
              <div class="col-4 text-end">
                <div class="icon icon-shape bg-gradient-primary shadow text-center border-radius-md">
                  <Icon name="mdi:cash" size="24" class="text-white opacity-10" />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <div class="col-xl-3 col-sm-6 mb-4">
        <div class="card">
          <div class="card-body p-3">
            <div class="row">
              <div class="col-8">
                <div class="numbers">
                  <MDTypography variant="caption" color="text" font-weight="bold" class="text-uppercase">
                    مستخدمين اليوم
                  </MDTypography>
                  <MDTypography variant="h5" font-weight="bold">
                    2,300
                  </MDTypography>
                  <MDTypography variant="caption" color="success" font-weight="bold">
                    <Icon name="mdi:arrow-up" size="16" />
                    3%
                  </MDTypography>
                </div>
              </div>
              <div class="col-4 text-end">
                <div class="icon icon-shape bg-gradient-info shadow text-center border-radius-md">
                  <Icon name="mdi:account-group" size="24" class="text-white opacity-10" />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <div class="col-xl-3 col-sm-6 mb-4">
        <div class="card">
          <div class="card-body p-3">
            <div class="row">
              <div class="col-8">
                <div class="numbers">
                  <MDTypography variant="caption" color="text" font-weight="bold" class="text-uppercase">
                    عملاء جدد
                  </MDTypography>
                  <MDTypography variant="h5" font-weight="bold">
                    +3,462
                  </MDTypography>
                  <MDTypography variant="caption" color="error" font-weight="bold">
                    <Icon name="mdi:arrow-down" size="16" />
                    2%
                  </MDTypography>
                </div>
              </div>
              <div class="col-4 text-end">
                <div class="icon icon-shape bg-gradient-success shadow text-center border-radius-md">
                  <Icon name="mdi:account-plus" size="24" class="text-white opacity-10" />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <div class="col-xl-3 col-sm-6 mb-4">
        <div class="card">
          <div class="card-body p-3">
            <div class="row">
              <div class="col-8">
                <div class="numbers">
                  <MDTypography variant="caption" color="text" font-weight="bold" class="text-uppercase">
                    مبيعات
                  </MDTypography>
                  <MDTypography variant="h5" font-weight="bold">
                    103,430 ر.س
                  </MDTypography>
                  <MDTypography variant="caption" color="success" font-weight="bold">
                    <Icon name="mdi:arrow-up" size="16" />
                    5%
                  </MDTypography>
                </div>
              </div>
              <div class="col-4 text-end">
                <div class="icon icon-shape bg-gradient-warning shadow text-center border-radius-md">
                  <Icon name="mdi:cart" size="24" class="text-white opacity-10" />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Content Cards RTL -->
    <div class="row">
      <div class="col-lg-7 mb-4">
        <div class="card">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              نظرة عامة على المبيعات
            </MDTypography>
          </div>
          <div class="card-body">
            <canvas id="sales-chart-rtl" height="170"></canvas>
            <MDTypography variant="caption" color="text" class="mt-3 d-block">
              <Icon name="mdi:clock-outline" size="16" class="me-1" />
              آخر تحديث قبل 4 دقائق
            </MDTypography>
          </div>
        </div>
      </div>
      
      <div class="col-lg-5 mb-4">
        <div class="card">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              الفئات
            </MDTypography>
          </div>
          <div class="card-body">
            <canvas id="pie-chart-rtl" height="220"></canvas>
          </div>
        </div>
      </div>
    </div>

    <!-- Table RTL -->
    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              الطلبات الأخيرة
            </MDTypography>
          </div>
          <div class="card-body">
            <div class="table-responsive">
              <table class="table align-items-center mb-0">
                <thead>
                  <tr>
                    <th class="text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">رقم الطلب</th>
                    <th class="text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">العميل</th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">التاريخ</th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">المبلغ</th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">الحالة</th>
                  </tr>
                </thead>
                <tbody>
                  <tr>
                    <td>
                      <MDTypography variant="button" font-weight="medium">
                        #10421
                      </MDTypography>
                    </td>
                    <td>
                      <div class="d-flex align-items-center">
                        <MDAvatar 
                          src="https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-1.jpg"
                          alt="customer"
                          size="sm"
                          class="me-2"
                        />
                        <MDTypography variant="button" font-weight="medium">
                          أحمد محمد
                        </MDTypography>
                      </div>
                    </td>
                    <td class="align-middle text-center">
                      <MDTypography variant="caption" color="text">
                        18 ديسمبر 2024
                      </MDTypography>
                    </td>
                    <td class="align-middle text-center">
                      <MDTypography variant="button" font-weight="bold">
                        1,250 ر.س
                      </MDTypography>
                    </td>
                    <td class="align-middle text-center">
                      <MDBadge color="success" variant="gradient">
                        مكتمل
                      </MDBadge>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <MDTypography variant="button" font-weight="medium">
                        #10422
                      </MDTypography>
                    </td>
                    <td>
                      <div class="d-flex align-items-center">
                        <MDAvatar 
                          src="https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-2.jpg"
                          alt="customer"
                          size="sm"
                          class="me-2"
                        />
                        <MDTypography variant="button" font-weight="medium">
                          فاطمة علي
                        </MDTypography>
                      </div>
                    </td>
                    <td class="align-middle text-center">
                      <MDTypography variant="caption" color="text">
                        17 ديسمبر 2024
                      </MDTypography>
                    </td>
                    <td class="align-middle text-center">
                      <MDTypography variant="button" font-weight="bold">
                        890 ر.س
                      </MDTypography>
                    </td>
                    <td class="align-middle text-center">
                      <MDBadge color="info" variant="gradient">
                        قيد المعالجة
                      </MDBadge>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'

definePageMeta({
  layout: 'default'
})

onMounted(() => {
  // Initialize charts with RTL support would go here
  console.log('RTL page mounted - charts would be initialized with RTL support')
})
</script>

<style scoped>
[dir="rtl"] {
  text-align: right;
}

[dir="rtl"] .text-end {
  text-align: left !important;
}

[dir="rtl"] .text-start {
  text-align: right !important;
}

[dir="rtl"] .me-1,
[dir="rtl"] .me-2,
[dir="rtl"] .me-3 {
  margin-right: 0 !important;
  margin-left: 0.25rem !important;
}

[dir="rtl"] .ms-auto {
  margin-left: 0 !important;
  margin-right: auto !important;
}

.icon {
  width: 3rem;
  height: 3rem;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 0.5rem;
}

.icon-shape {
  width: 3rem;
  height: 3rem;
}

.bg-gradient-primary {
  background-image: linear-gradient(195deg, #5e72e4 0%, #825ee4 100%);
}

.bg-gradient-info {
  background-image: linear-gradient(195deg, #49a3f1 0%, #1A73E8 100%);
}

.bg-gradient-success {
  background-image: linear-gradient(195deg, #66BB6A 0%, #43A047 100%);
}

.bg-gradient-warning {
  background-image: linear-gradient(195deg, #FFA726 0%, #FB8C00 100%);
}

.border-radius-md {
  border-radius: 0.5rem;
}

.opacity-10 {
  opacity: 0.1;
}
</style>
