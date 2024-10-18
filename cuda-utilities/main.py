import torch

def test_cuda():
    if torch.cuda.is_available():
        print("CUDA is available!")
        print(f"CUDA version: {torch.version.cuda}")
        print(f"Number of CUDA devices: {torch.cuda.device_count()}")
        print(f"CUDA device name: {torch.cuda.get_device_name(0)}")
        
        # Perform a simple tensor operation on GPU to verify CUDA functionality
        tensor = torch.tensor([1.0, 2.0, 3.0], device='cuda')
        print(f"Tensor on GPU: {tensor}")
    else:
        print("CUDA is not available!")

if __name__ == "__main__":
    test_cuda()
