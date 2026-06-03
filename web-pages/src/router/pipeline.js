import Index from '@/views/pipeline/Index'
import Create from '@/views/pipeline/Create'
import Edit from '@/views/pipeline/Edit'

export default [
  {
    path: '/pipeline',
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
