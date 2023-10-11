using UnityEngine;

// �t���[�c�𐧌䂷��X�N���v�g
public class Fruit : MonoBehaviour
{
	// �l�����o�̃v���n�u
	public GameObject m_collectedPrefab;

	// �t���[�c����������ɍĐ����� SE
	public AudioClip m_collectedClip;

	// ���̃I�u�W�F�N�g�Ɠ����������ɌĂяo�����֐�
	private void OnTriggerEnter2D( Collider2D other )
	{
		// ���O�ɁuPlayer�v���܂܂��I�u�W�F�N�g�Ɠ���������
		if ( other.name.Contains( "Player" ) )
		{
				// �l�����o�̃I�u�W�F�N�g���쐬����
			var collected = Instantiate
			(
				m_collectedPrefab,
				transform.position,
				Quaternion.identity
			);

			// �l�����o�̃I�u�W�F�N�g����A�j���[�^�[�̏����擾����
			var animator = collected.GetComponent<Animator>();

			// ���ݍĐ����̃A�j���[�V�����̏����擾����
			var info = animator.GetCurrentAnimatorStateInfo( 0 );

			// ���ݍĐ����̃A�j���[�V�����̍Đ����ԁi�b�j���擾����
			var time = info.length;

			// �A�j���[�V�����̍Đ�������������
			// �l�����o���폜����悤�ɓo�^����
			Destroy( collected, time );

			// �������g���폜����
			Destroy( gameObject );

			GameManager.instance.PlayerScore();

			// �t���[�c����������� SE ���Đ�����
			var audioSource = FindObjectOfType<AudioSource>();
			audioSource.PlayOneShot( m_collectedClip );
		}
	}
}