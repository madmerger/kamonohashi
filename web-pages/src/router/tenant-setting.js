import ManageTenant from '@/views/tenant-setting/tenant/Setting'
import ManageUserIndex from '@/views/tenant-setting/user/Index'
import ManageUserEdit from '@/views/tenant-setting/user/Edit'
import ManageResourceIndex from '@/views/tenant-setting/resource/Index'
import ManageResourceNode from '@/views/tenant-setting/resource/Node'
import ManageResourceContainerList from '@/views/tenant-setting/resource/ContainerList'
import ManageResourceEdit from '@/views/tenant-setting/resource/Edit'
import ManageProjectIndex from '@/views/tenant-setting/project/Index'
import ManageProjectEdit from '@/views/tenant-setting/project/Edit'
import ManageCustomRoleIndex from '@/views/tenant-setting/custom-role/Index'
import ManageCustomRoleEdit from '@/views/tenant-setting/custom-role/Edit'
import ManagePermissionIndex from '@/views/tenant-setting/permission/Index'

export default [
  {
    path: '/manage/tenant',
    component: ManageTenant,
  },
  {
    path: '/manage/user',
    component: ManageUserIndex,
    children: [
      {
        path: ':id',
        component: ManageUserEdit,
        props: true,
      },
    ],
  },
  {
    path: '/manage/resource',
    component: ManageResourceIndex,
    children: [
      {
        path: '',
        component: ManageResourceNode,
        children: [
          {
            path: ':nodeName/:name',
            component: ManageResourceEdit,
            props: true,
          },
        ],
      },
      {
        path: 'container-list',
        component: ManageResourceContainerList,
        children: [
          {
            path: ':nodeName/:name',
            component: ManageResourceEdit,
            props: true,
          },
        ],
      },
    ],
  },
  {
    path: '/manage/project',
    component: ManageProjectIndex,
    children: [
      {
        path: ':id',
        component: ManageProjectEdit,
        props: true,
      },
    ],
  },
  {
    path: '/manage/custom-role',
    component: ManageCustomRoleIndex,
    children: [
      {
        path: ':id',
        component: ManageCustomRoleEdit,
        props: true,
      },
    ],
  },
  {
    path: '/manage/permission',
    component: ManagePermissionIndex,
  },
]
