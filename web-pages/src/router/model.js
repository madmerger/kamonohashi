import Index from '@/views/model/Index'
import Edit from '@/views/model/Edit'
import Create from '@/views/model/Create'

export default [
  {
    path: '/model',
    component: Index,
    children: [
      {
        path: 'create',
        component: Create,
      },
      {
        path: ':id',
        component: Edit,
        props: true,
      },
    ],
  },
]
